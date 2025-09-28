using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Bll.Services;

public partial class SyncService
{
    public async Task SyncGroundwaterSiteAsync()
    {
        _logger.LogInformation("開始同步地下水監測站及採樣數據");

        // 1. 同步地下水監測站基本資料
        var siteCount = await SyncGroundwaterSitesAsync();
        _logger.LogInformation("地下水監測站同步完成，共處理 {Count} 條記錄", siteCount);

        // 2. 同步監測採樣數據（避免N+1查詢）
        var sampleCount = await SyncGroundwaterSamplesAsync();
        _logger.LogInformation("地下水採樣數據同步完成，共處理 {Count} 條記錄", sampleCount);

        _logger.LogInformation("地下水監測站及採樣數據同步完成");
    }

    /// <summary>
    /// 同步地下水監測站基本資料
    /// </summary>
    private async Task<int> SyncGroundwaterSitesAsync()
    {
        _logger.LogInformation("開始同步地下水監測站基本資料");

        // 獲取總數
        var firstResponse = await _moenvApiService.GetWqxP07DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何地下水監測站數據");
            return 0;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return 0;

        _logger.LogInformation("總共需要處理 {Total} 條地下水監測站記錄", totalRecords);

        const int batchSize = 1000;
        var processedCount = 0;

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 條監測站記錄", 
                offset, Math.Min(offset + batchSize, totalRecords));

            var response = await _moenvApiService.GetWqxP07DataAsync(offset, batchSize);
            if (response?.Records == null || !response.Records.Any()) continue;

            var records = response.Records.ToList();
            var siteIds = records.Select(r => r.Siteid).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();

            // 批量查詢已存在的監測站
            var existingSites = await _dbContext.GroundwaterSites
                .Where(s => siteIds.Contains(s.SiteId))
                .ToDictionaryAsync(s => s.SiteId);

            var newSites = new List<GroundwaterSite>();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.Siteid)) continue;

                // 驗證坐標
                if (!string.IsNullOrEmpty(record.Twd97Lat) && !string.IsNullOrEmpty(record.Twd97Lon))
                {
                    var result = ValidateHelper.ParseAndCorrectCoordinate(record.Twd97Lat, record.Twd97Lon);
                    if (!result.IsValid)
                    {
                        _logger.LogWarning("{SiteId} -> 經緯度不合法或無法解析: Lat='{LatStr}', Lon='{LonStr}'", 
                            record.Siteid, record.Twd97Lat, record.Twd97Lon);
                        continue;
                    }
                    record.Twd97Lat = result.Latitude.ToString();
                    record.Twd97Lon = result.Longitude.ToString();
                }

                // 數據驗證
                var validationResult = await _validationService.ValidateAsync(record);
                if (!validationResult.IsValid)
                {
                    PrintErrorRecord(record);
                    _logger.LogWarning("地下水監測站數據驗證失敗 - SiteId: {SiteId}, 錯誤: {Errors}", 
                        record.Siteid, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    continue;
                }

                if (!existingSites.TryGetValue(record.Siteid, out var site))
                {
                    site = new GroundwaterSite
                    {
                        Id = Guid.NewGuid(),
                        SiteId = record.Siteid,
                        SiteName = record.Sitename,
                        SiteEngName = record.Siteengname,
                        UgwDistrictName = record.Ugwdistname,
                        County = record.County,
                        Township = record.Township,
                        Statusofuse = record.Statusofuse,
                        Twd97Lon = decimal.TryParse(record.Twd97Lon, out var lon) ? lon : null,
                        Twd97Lat = decimal.TryParse(record.Twd97Lat, out var lat) ? lat : null,
                        Twd97Tm2X = decimal.TryParse(record.Twd97Tm2X, out var x) ? x : null,
                        Twd97Tm2Y = decimal.TryParse(record.Twd97Tm2Y, out var y) ? y : null
                    };
                    newSites.Add(site);
                    existingSites[record.Siteid] = site;
                }
                else
                {
                    // 更新現有監測站信息
                    site.SiteName = record.Sitename ?? site.SiteName;
                    site.SiteEngName = record.Siteengname ?? site.SiteEngName;
                    site.UgwDistrictName = record.Ugwdistname ?? site.UgwDistrictName;
                    site.County = record.County ?? site.County;
                    site.Township = record.Township ?? site.Township;
                    site.Statusofuse = record.Statusofuse ?? site.Statusofuse;
                    site.Twd97Lon = decimal.TryParse(record.Twd97Lon, out var lon) ? lon : site.Twd97Lon;
                    site.Twd97Lat = decimal.TryParse(record.Twd97Lat, out var lat) ? lat : site.Twd97Lat;
                    site.Twd97Tm2X = decimal.TryParse(record.Twd97Tm2X, out var x) ? x : site.Twd97Tm2X;
                    site.Twd97Tm2Y = decimal.TryParse(record.Twd97Tm2Y, out var y) ? y : site.Twd97Tm2Y;
                }
            }

            // 批量插入新數據
            if (newSites.Any())
                _dbContext.GroundwaterSites.AddRange(newSites);

            await _dbContext.SaveChangesAsync();
            processedCount += records.Count;
        }

        return processedCount;
    }

    /// <summary>
    /// 同步地下水採樣數據，避免N+1查詢問題
    /// </summary>
    private async Task<int> SyncGroundwaterSamplesAsync()
    {
        _logger.LogInformation("開始同步地下水採樣數據");

        // 獲取總數
        var firstResponse = await _moenvApiService.GetWqxP02DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何地下水採樣數據");
            return 0;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return 0;

        _logger.LogInformation("總共需要處理 {Total} 條地下水採樣記錄", totalRecords);

        const int batchSize = 1000;
        var processedCount = 0;

        // 預先加載所有監測站ID映射，避免N+1查詢
        var siteIdMap = await _dbContext.GroundwaterSites
            .Select(s => new { s.Id, s.SiteId })
            .ToDictionaryAsync(x => x.SiteId, x => x.Id);

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 條採樣記錄", 
                offset, Math.Min(offset + batchSize, totalRecords));

            var response = await _moenvApiService.GetWqxP02DataAsync(offset, batchSize);
            if (response?.Records == null || !response.Records.Any()) continue;

            var records = response.Records.ToList();
            var newSamples = new List<GroundwaterSiteSample>();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.Siteid) || string.IsNullOrEmpty(record.Sampledate))
                    continue;

                // 檢查對應的監測站是否存在
                if (!siteIdMap.TryGetValue(record.Siteid, out var groundwaterSiteId))
                {
                    _logger.LogWarning("找不到對應的地下水監測站 {SiteId}，跳過採樣記錄", record.Siteid);
                    continue;
                }

                // 解析採樣日期
                if (!DateTime.TryParse(record.Sampledate, out var sampleDate))
                {
                    _logger.LogWarning("無法解析採樣日期 {SampleDate}，跳過記錄", record.Sampledate);
                    continue;
                }

                // 數據驗證
                var validationResult = await _validationService.ValidateAsync(record);
                if (!validationResult.IsValid)
                {
                    PrintErrorRecord(record);
                    _logger.LogWarning("地下水採樣數據驗證失敗 - SiteId: {SiteId}, 錯誤: {Errors}", 
                        record.Siteid, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    continue;
                }

                newSamples.Add(new GroundwaterSiteSample
                {
                    Id = Guid.NewGuid(),
                    SampleDate = sampleDate,
                    ItemName = record.Itemname,
                    ItemEngName = record.Itemengname,
                    ItemEngAbbreviation = record.Itemengabbreviation,
                    ItemValue = decimal.TryParse(record.Itemvalue, out var value) ? value : null,
                    ItemUnit = record.Itemunit,
                    Note = record.Note,
                    GroundwaterSiteId = groundwaterSiteId
                });
            }

            if (newSamples.Any())
            {
                _dbContext.GroundwaterSiteSamples.AddRange(newSamples);
            }

            await _dbContext.SaveChangesAsync();
            processedCount += records.Count;
        }

        return processedCount;
    }
}