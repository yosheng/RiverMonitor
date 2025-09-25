using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Dal;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Bll.Services;

public partial class SyncService
{
    /// <summary>
    /// 同步河川水質監測站及監測資料
    /// 1. 先同步監測站基本資料 (WqxP06)
    /// 2. 再同步監測資料 (WqxP01)，避免N+1查詢問題
    /// </summary>
    public async Task SyncMonitoringSiteAsync()
    {
        _logger.LogInformation("開始同步河川水質監測資料");

        // 1. 同步監測站基本資料
        var siteCount = await SyncMonitoringSitesAsync();
        _logger.LogInformation("監測站基本資料同步完成，共處理 {Count} 個監測站", siteCount);

        // 2. 同步監測資料
        var sampleCount = await SyncMonitoringSamplesAsync();
        _logger.LogInformation("監測資料同步完成，共處理 {Count} 筆監測資料", sampleCount);

        _logger.LogInformation("河川水質監測資料同步完成");
    }

    /// <summary>
    /// 同步監測站基本資料
    /// </summary>
    private async Task<int> SyncMonitoringSitesAsync()
    {
        _logger.LogInformation("開始同步監測站基本資料");

        // 獲取總數
        var firstResponse = await _moenvApiService.GetWqxP06DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何監測站資料");
            return 0;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return 0;

        _logger.LogInformation("總共需要處理 {Total} 個監測站", totalRecords);

        const int batchSize = 1000;
        var processedCount = 0;

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 個監測站", 
                offset, Math.Min(offset + batchSize, totalRecords));

            await ProcessMonitoringSitesBatchAsync(offset, batchSize);
            processedCount += batchSize;
        }

        return processedCount;
    }

    /// <summary>
    /// 處理單一批次的監測站資料
    /// </summary>
    private async Task ProcessMonitoringSitesBatchAsync(int offset, int limit)
    {
        var response = await _moenvApiService.GetWqxP06DataAsync(offset, limit);
        if (response?.Records == null || !response.Records.Any()) return;

        var records = response.Records.ToList();
        var siteIds = records.Select(r => r.Siteid).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        
        // 批量查詢已存在的監測站
        var existingSites = await _dbContext.MonitoringSites
            .Where(s => siteIds.Contains(s.SiteId))
            .ToDictionaryAsync(s => s.SiteId);

        var newSites = new List<MonitoringSite>();
        var validRecords = 0;
        var invalidRecords = 0;

        foreach (var record in records)
        {
            if (string.IsNullOrEmpty(record.Siteid)) continue;

            // 坐標驗證與轉換
            decimal? twd97Lon = null, twd97Lat = null, twd97Tm2X = null, twd97Tm2Y = null;
            
            if (!string.IsNullOrEmpty(record.Twd97Lon) && !string.IsNullOrEmpty(record.Twd97Lat))
            {
                var result = ValidateHelper.ParseAndCorrectCoordinate(record.Twd97Lat, record.Twd97Lon);
                if (result.IsValid)
                {
                    twd97Lon = result.Longitude;
                    twd97Lat = result.Latitude;
                }
                else
                {
                    _logger.LogWarning("{SiteId} -> TWD97經緯度坐標校正失敗: Lat='{LatStr}', Lon='{LonStr}'", 
                        record.Siteid, record.Twd97Lat, record.Twd97Lon);
                }
            }

            // TM2坐標轉換
            if (!string.IsNullOrEmpty(record.Twd97Tm2X))
                twd97Tm2X = decimal.TryParse(record.Twd97Tm2X, out var x) ? x : null;
            
            if (!string.IsNullOrEmpty(record.Twd97Tm2Y))
                twd97Tm2Y = decimal.TryParse(record.Twd97Tm2Y, out var y) ? y : null;
            
            // 數據驗證
            var validationResult = await _validationService.ValidateAsync(record);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("監測站資料驗證失敗 - SiteId: {SiteId}, 錯誤: {Errors}", 
                    record.Siteid, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                invalidRecords++;
                continue;
            }

            validRecords++;

            if (!existingSites.TryGetValue(record.Siteid, out var site))
            {
                site = new MonitoringSite
                {
                    Id = Guid.NewGuid(),
                    SiteId = record.Siteid,
                    SiteName = record.Sitename,
                    SiteEngName = record.Siteengname,
                    County = record.County,
                    Township = record.Township,
                    Basin = record.Basin,
                    River = record.River,
                    Twd97Lon = twd97Lon,
                    Twd97Lat = twd97Lat,
                    Twd97Tm2X = twd97Tm2X,
                    Twd97Tm2Y = twd97Tm2Y,
                    Statusofuse = record.Statusofuse
                };
                newSites.Add(site);
                existingSites[record.Siteid] = site;
            }
            else
            {
                // 更新現有監測站信息
                site.SiteName = record.Sitename ?? site.SiteName;
                site.SiteEngName = record.Siteengname ?? site.SiteEngName;
                site.County = record.County ?? site.County;
                site.Township = record.Township ?? site.Township;
                site.Basin = record.Basin ?? site.Basin;
                site.River = record.River ?? site.River;
                site.Twd97Lon = twd97Lon ?? site.Twd97Lon;
                site.Twd97Lat = twd97Lat ?? site.Twd97Lat;
                site.Twd97Tm2X = twd97Tm2X ?? site.Twd97Tm2X;
                site.Twd97Tm2Y = twd97Tm2Y ?? site.Twd97Tm2Y;
                site.Statusofuse = record.Statusofuse ?? site.Statusofuse;
            }
        }

        _logger.LogInformation("監測站批次處理完成 - 有效記錄: {ValidRecords}, 無效記錄: {InvalidRecords}", 
            validRecords, invalidRecords);

        if (newSites.Any())
        {
            _dbContext.MonitoringSites.AddRange(newSites);
        }

        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 同步監測資料，避免N+1查詢問題
    /// </summary>
    private async Task<int> SyncMonitoringSamplesAsync()
    {
        _logger.LogInformation("開始同步監測資料");

        // 獲取總數
        var firstResponse = await _moenvApiService.GetWqxP01DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何監測資料");
            return 0;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return 0;

        _logger.LogInformation("總共需要處理 {Total} 筆監測資料", totalRecords);

        const int batchSize = 1000;
        var processedCount = 0;

        // 預先加載所有監測站ID映射，避免N+1查詢
        var siteIdMap = await _dbContext.MonitoringSites
            .Select(s => new { s.Id, s.SiteId })
            .ToDictionaryAsync(x => x.SiteId, x => x.Id);

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 筆監測資料", 
                offset, Math.Min(offset + batchSize, totalRecords));

            await ProcessMonitoringSamplesBatchAsync(offset, batchSize, siteIdMap);
            processedCount += batchSize;
        }

        return processedCount;
    }

    /// <summary>
    /// 處理單一批次的監測資料
    /// </summary>
    private async Task ProcessMonitoringSamplesBatchAsync(int offset, int limit, Dictionary<string, Guid> siteIdMap)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RiverMonitorDbContext>();
        var apiService = scope.ServiceProvider.GetRequiredService<IMoenvApiService>();
        var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
        
        var response = await apiService.GetWqxP01DataAsync(offset, limit);
        if (response?.Records == null || !response.Records.Any()) return;

        var records = response.Records.ToList();
        
        // 生成唯一標識符來避免重複 (SiteId + SampleDate + ItemName)
        var sampleKeys = records.Select(r => $"{r.Siteid}_{r.Sampledate}_{r.Itemname}")
            .Where(key => !string.IsNullOrEmpty(key) && !key.EndsWith("_"))
            .Distinct()
            .ToList();
        
        // 批量查詢已存在的樣本
        var existingSamples = await dbContext.MonitoringSiteSamples
            .Where(s => sampleKeys.Contains($"{s.Site.SiteId}_{s.SampleDate:yyyy-MM-dd}_{s.ItemName}"))
            .Select(s => new { Key = $"{s.Site.SiteId}_{s.SampleDate:yyyy-MM-dd}_{s.ItemName}", Sample = s })
            .ToDictionaryAsync(x => x.Key, x => x.Sample);

        var newSamples = new List<MonitoringSiteSample>();
        var validRecords = 0;
        var invalidRecords = 0;

        foreach (var record in records)
        {
            if (string.IsNullOrEmpty(record.Siteid) || string.IsNullOrEmpty(record.Sampledate) || string.IsNullOrEmpty(record.Itemname))
            {
                invalidRecords++;
                continue;
            }

            // 檢查對應的監測站是否存在
            if (!siteIdMap.TryGetValue(record.Siteid, out var monitoringSiteId))
            {
                _logger.LogWarning("找不到對應的監測站 {SiteId}，跳過樣本記錄", record.Siteid);
                invalidRecords++;
                continue;
            }

            // 數據驗證
            var validationResult = await validationService.ValidateAsync(record);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("監測資料驗證失敗 - SiteId: {SiteId}, SampleDate: {SampleDate}, Item: {ItemName}, 錯誤: {Errors}", 
                    record.Siteid, record.Sampledate, record.Itemname, 
                    string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                invalidRecords++;
                continue;
            }

            // 解析採樣日期
            if (!DateTime.TryParse(record.Sampledate, out var sampleDate))
            {
                _logger.LogWarning("採樣日期格式無效 - SiteId: {SiteId}, Date: {SampleDate}", 
                    record.Siteid, record.Sampledate);
                invalidRecords++;
                continue;
            }

            var sampleKey = $"{record.Siteid}_{sampleDate:yyyy-MM-dd}_{record.Itemname}";
            
            // 解析檢測值
            var itemValue = decimal.TryParse(record.Itemvalue, out var value) ? value : (decimal?)null;

            validRecords++;

            if (!existingSamples.TryGetValue(sampleKey, out var sample))
            {
                sample = new MonitoringSiteSample
                {
                    Id = Guid.NewGuid(),
                    SampleDate = sampleDate,
                    ItemName = record.Itemname,
                    ItemEngName = record.Itemengname,
                    ItemEngAbbreviation = record.Itemengabbreviation,
                    ItemValue = itemValue,
                    ItemUnit = record.Itemunit,
                    Note = record.Note,
                    MonitoringSiteId = monitoringSiteId
                };
                newSamples.Add(sample);
                existingSamples[sampleKey] = sample;
            }
            else
            {
                // 更新現有樣本信息
                sample.SampleDate = sampleDate;
                sample.ItemName = record.Itemname ?? sample.ItemName;
                sample.ItemEngName = record.Itemengname ?? sample.ItemEngName;
                sample.ItemEngAbbreviation = record.Itemengabbreviation ?? sample.ItemEngAbbreviation;
                sample.ItemValue = itemValue ?? sample.ItemValue;
                sample.ItemUnit = record.Itemunit ?? sample.ItemUnit;
                sample.Note = record.Note ?? sample.Note;
                sample.MonitoringSiteId = monitoringSiteId;
            }
        }

        _logger.LogInformation("監測資料批次處理完成 - 有效記錄: {ValidRecords}, 無效記錄: {InvalidRecords}", 
            validRecords, invalidRecords);

        if (newSamples.Any())
        {
            dbContext.MonitoringSiteSamples.AddRange(newSamples);
        }

        await dbContext.SaveChangesAsync();
    }
}