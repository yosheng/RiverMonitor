using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using Hangfire.RecurringJobAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Bll.Helpers;
using RiverMonitor.Dal;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Bll.Services;

public interface ISyncService
{
    [RecurringJob("0 2 * * *", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-wastewater-emission")]
    Task SyncWastewaterEmissionAsync();
    
    [RecurringJob("0 3 * * *", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-pollution-site-announcement")]
    Task SyncPollutionSiteAndAnnouncementAsync();

    [RecurringJob("0 4 * * *", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-monitoring-site")]
    Task SyncMonitoringSiteAsync();
    
    [RecurringJob("0 5 * * *", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-groundwater-site")]
    Task SyncGroundwaterSiteAsync();
    
    [RecurringJob("0 1 * * SUN", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-irrigation-agency")]
    Task SyncIrrigationAgencyAsync();
    
    [RecurringJob("30 1 * * SUN", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-irrigation-agency-station")]
    Task SyncIrrigationAgencyStationAsync();
    
    [RecurringJob("0 6 * * *", ApplicationConstant.TimeZone, "default", RecurringJobId = "sync-irrigation-agency-station-monitoring-data")]
    Task SyncIrrigationAgencyStationMonitoringDataAsync();
}

public partial class SyncService : ISyncService
{
    private readonly IMoenvApiService _moenvApiService;
    private readonly RiverMonitorDbContext _dbContext;
    private readonly ILogger<SyncService> _logger;
    private readonly IValidationService _validationService;
    private readonly IMoaApiService _moaApiService;
    private readonly IIaApiService _iaApiService;
    private readonly IServiceProvider _serviceProvider;

    public SyncService(
        IMoenvApiService moenvApiService,
        RiverMonitorDbContext dbContext,
        ILogger<SyncService> logger, 
        IValidationService validationService,
        IServiceProvider serviceProvider, IMoaApiService moaApiService, IIaApiService iaApiService)
    {
        _moenvApiService = moenvApiService;
        _dbContext = dbContext;
        _logger = logger;
        _validationService = validationService;
        _serviceProvider = serviceProvider;
        _moaApiService = moaApiService;
        _iaApiService = iaApiService;
    }

    public async Task SyncWastewaterEmissionAsync()
    {
        _logger.LogInformation("開始同步廢水排放數據");

        // 獲取總數
        var firstResponse = await _moenvApiService.GetEmsS03DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何數據");
            return;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return;

        _logger.LogInformation("總共需要處理 {Total} 條記錄", totalRecords);

        const int batchSize = 10000;
        var processedCount = 0;

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 條記錄", offset, Math.Min(offset + batchSize, totalRecords));

            var response = await _moenvApiService.GetEmsS03DataAsync(offset, batchSize);
            if (response?.Records == null || !response.Records.Any()) continue;

            var records = response.Records.ToList();

            // 獲取所有涉及的EmsNo
            var emsNos = records.Select(r => r.EmsNo).Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList();

            // 批量查詢已存在的許可證
            var existingPermits = await _dbContext.WastewaterPermits
                .Where(p => emsNos.Contains(p.EmsNo))
                .ToDictionaryAsync(p => p.EmsNo);

            var newPermits = new List<WastewaterPermit>();
            var newEmissions = new List<PollutantEmission>();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.EmsNo)) continue;
                
                var result = ValidateHelper.ParseAndCorrectCoordinate(record.LetNorth, record.LetEast);

                if (!result.IsValid)
                {
                    _logger.LogWarning("{EmsNo} -> 經緯度不合法或無法解析: Lat='{LatStr}', Lon='{LonStr}'", 
                        record.EmsNo, record.LetNorth, record.LetEast);
                    continue;
                }
                
                record.LetNorth = result.Latitude.ToString();
                record.LetEast = result.Longitude.ToString();

                // 處理統一編號不合法
                if (!string.IsNullOrEmpty(record.Unino) && 
                    !Regex.IsMatch(record.Unino, @"^\d{8}$"))
                {
                    _logger.LogWarning("{RecordEmsNo} -> {RecordUnino} 統一編號不合法", record.EmsNo, record.Unino);
                    continue;
                }

                var validationResult = await _validationService.ValidateAsync(record);

                if (!validationResult.IsValid)
                {
                    PrintErrorRecord(record);
                    _logger.LogWarning("數據驗證失敗：{ValidationError}",
                        string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new ArgumentException("處理錯誤");
                }

                // 處理許可證（去重）
                if (!existingPermits.TryGetValue(record.EmsNo, out var permit))
                {
                    permit = new WastewaterPermit
                    {
                        Id = Guid.NewGuid(),
                        EmsNo = record.EmsNo,
                        FacilityName = record.FacName ?? "",
                        Address = record.Address ?? "",
                        UniformNo = record.Unino,
                        PermitNo = record.PerNo,
                        PermitType = record.PerType,
                        OutletId = record.Let,
                        OutletWaterType = record.LetWatertype,
                        PermitStartDate = DateTime.TryParse(record.PerSdate, out var sdate) ? sdate : null,
                        PermitEndDate = DateTime.TryParse(record.PerEdate, out var edate) ? edate : null,
                        PermittedWaterVolume = decimal.TryParse(record.PerWater, out var pwater) ? pwater : null,
                        OutletTm2x = decimal.TryParse(record.LetTm2x, out var x) ? x : null,
                        OutletTm2y = decimal.TryParse(record.LetTm2y, out var y) ? y : null,
                        OutletLongitude = decimal.TryParse(record.LetEast, out var lon) ? lon : null,
                        OutletLatitude = decimal.TryParse(record.LetNorth, out var lat) ? lat : null
                    };
                    newPermits.Add(permit);
                    existingPermits[record.EmsNo] = permit;
                }
                else
                {
                    // 更新許可證信息
                    permit.FacilityName = record.FacName ?? permit.FacilityName;
                    permit.Address = record.Address ?? permit.Address;
                    permit.UniformNo = record.Unino ?? permit.UniformNo;
                    permit.PermitNo = record.PerNo ?? permit.PermitNo;
                    permit.PermitType = record.PerType ?? permit.PermitType;
                    permit.OutletId = record.Let ?? permit.OutletId;
                    permit.OutletWaterType = record.LetWatertype ?? permit.OutletWaterType;
                    if (DateTime.TryParse(record.PerSdate, out var sdate)) permit.PermitStartDate = sdate;
                    if (DateTime.TryParse(record.PerEdate, out var edate)) permit.PermitEndDate = edate;
                    if (decimal.TryParse(record.PerWater, out var pwater)) permit.PermittedWaterVolume = pwater;
                    if (decimal.TryParse(record.LetTm2x, out var x)) permit.OutletTm2x = x;
                    if (decimal.TryParse(record.LetTm2y, out var y)) permit.OutletTm2y = y;
                    if (decimal.TryParse(record.LetEast, out var lon)) permit.OutletLongitude = lon;
                    if (decimal.TryParse(record.LetNorth, out var lat)) permit.OutletLatitude = lat;
                }

                // 污染物排放記錄完整保存（不去重）
                if (!string.IsNullOrEmpty(record.EmiItem) || !string.IsNullOrEmpty(record.EmiWater))
                {
                    newEmissions.Add(new PollutantEmission
                    {
                        Id = Guid.NewGuid(),
                        WastewaterPermitId = permit.Id,
                        EmissionItemName = record.EmiItem ?? "未知項目",
                        EmissionStartDate = DateTime.TryParse(record.EmiSdate, out var emisdate) ? emisdate : null,
                        EmissionEndDate = DateTime.TryParse(record.EmiEdate, out var emiedate) ? emiedate : null,
                        EmissionWaterVolume = decimal.TryParse(record.EmiWater, out var emiwater) ? emiwater : null,
                        EmissionWaterUnit = record.EmiWaterunit,
                        EmissionValue = decimal.TryParse(record.EmiValue, out var emivalue) ? emivalue : null,
                        EmissionUnit = record.EmiUnits,
                        TotalItemValue = decimal.TryParse(record.ItemValue, out var itemvalue) ? itemvalue : null,
                        TotalItemUnit = record.ItemUnits
                    });
                }
            }

            // 批量插入新數據
            if (newPermits.Any())
                _dbContext.WastewaterPermits.AddRange(newPermits);

            if (newEmissions.Any())
                _dbContext.PollutantEmissions.AddRange(newEmissions);

            await _dbContext.SaveChangesAsync();
            processedCount += records.Count;
        }

        _logger.LogInformation("廢水排放數據同步完成，總共處理 {Count} 條記錄", processedCount);
    }

    public async Task SyncPollutionSiteAndAnnouncementAsync()
    {
        _logger.LogInformation("開始同步污染場址及公告數據");

        // 1. 同步污染場址
        var siteCount = await SyncPollutionSitesConcurrentlyAsync();
        
        _logger.LogInformation("污染場址同步完成，共處理 {Count} 條記錄", siteCount);

        // 2. 同步公告（避免N+1查詢）
        var announcementCount = await SyncAnnouncementsAsync();
        
        _logger.LogInformation("公告同步完成，共處理 {Count} 條記錄", announcementCount);
        _logger.LogInformation("污染場址及公告數據同步完成");
    }

    /// <summary>
    /// 使用並發方式同步污染場址數據
    /// </summary>
    private async Task<int> SyncPollutionSitesConcurrentlyAsync()
    {
        // 獲取總數
        var firstResponse = await _moenvApiService.GetEmsS07DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何污染場址數據");
            return 0;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return 0;

        _logger.LogInformation("總共需要處理 {Total} 條污染場址記錄", totalRecords);

        const int batchSize = 5000;
        const int maxConcurrency = 10;
        var processedCount = 0;

        // 如果數據量小於1萬，使用單線程處理
        if (totalRecords < 10000)
        {
            return await SyncPollutionSitesInBatchesAsync(totalRecords, batchSize);
        }

        // 大數據量使用並發處理
        var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        var tasks = new List<Task>();

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            var currentOffset = offset;
            
            await semaphore.WaitAsync();
            
            var task = Task.Run(async () =>
            {
                try
                {
                    await ProcessPollutionSiteBatchAsync(currentOffset, batchSize);
                    Interlocked.Add(ref processedCount, batchSize);
                    _logger.LogInformation("已處理 {Processed}/{Total} 條污染場址記錄", 
                        Math.Min(processedCount, totalRecords), totalRecords);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        return processedCount;
    }

    /// <summary>
    /// 單線程批次處理污染場址
    /// </summary>
    private async Task<int> SyncPollutionSitesInBatchesAsync(int totalRecords, int batchSize)
    {
        var processedCount = 0;
        
        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 條污染場址記錄", 
                offset, Math.Min(offset + batchSize, totalRecords));
            
            await ProcessPollutionSiteBatchAsync(offset, batchSize);
            processedCount += batchSize;
        }
        
        return processedCount;
    }

    /// <summary>
    /// 處理單一批次的污染場址數據
    /// </summary>
    private async Task ProcessPollutionSiteBatchAsync(int offset, int limit)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RiverMonitorDbContext>();
        var apiService = scope.ServiceProvider.GetRequiredService<IMoenvApiService>();
        var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();
        
        var response = await apiService.GetEmsS07DataAsync(offset, limit);
        if (response?.Records == null || !response.Records.Any()) return;

        var records = response.Records.ToList();
        var siteIds = records.Select(r => r.SiteId).Where(id => !string.IsNullOrEmpty(id)).Distinct().ToList();
        
        // 批量查詢已存在的場址
        var existingSites = await dbContext.PollutionSites
            .Where(s => siteIds.Contains(s.SiteId))
            .ToDictionaryAsync(s => s.SiteId);

        var newSites = new List<PollutionSite>();
        var validRecords = 0;
        var invalidRecords = 0;

        foreach (var record in records)
        {
            if (string.IsNullOrEmpty(record.SiteId)) continue;

            // 驗證坐標
            if (!string.IsNullOrEmpty(record.Wgs84Lat) && !string.IsNullOrEmpty(record.Wgs84Lng))
            {
                var result = ValidateHelper.ParseAndCorrectCoordinate(record.Wgs84Lat, record.Wgs84Lng);
                if (!result.IsValid)
                {
                    _logger.LogWarning("{SiteId} -> 經緯度不合法或無法解析: Lat='{LatStr}', Lon='{LonStr}'", 
                        record.SiteId, record.Wgs84Lat, record.Wgs84Lng);
                    continue;
                }
                record.Wgs84Lat = result.Latitude.ToString();
                record.Wgs84Lng = result.Longitude.ToString();
            }
            
            // 數據驗證
            var validationResult = await validationService.ValidateAsync(record);
            if (!validationResult.IsValid)
            {
                PrintErrorRecord(record);
                _logger.LogWarning("污染場址數據驗證失敗 - SiteId: {SiteId}, 錯誤: {Errors}", 
                    record.SiteId, string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                invalidRecords++;
                continue;
            }

            validRecords++;

            if (!existingSites.TryGetValue(record.SiteId, out var site))
            {
                site = new PollutionSite
                {
                    Id = Guid.NewGuid(),
                    SiteId = record.SiteId,
                    SiteName = record.SiteName,
                    County = record.County,
                    Township = record.Township,
                    Address = record.Pollutantaddress,
                    SiteType = record.SiteType,
                    SiteUse = record.SiteUse,
                    Pollutants = record.Pollutant,
                    ControlType = record.Controltype,
                    SiteArea = decimal.TryParse(record.Sitearea, out var area) ? area : null,
                    LandLots = record.Landno,
                    Dtmx = decimal.TryParse(record.Dtmx, out var dtmx) ? dtmx : null,
                    Dtmy = decimal.TryParse(record.Dtmy, out var dtmy) ? dtmy : null,
                    Longitude = decimal.TryParse(record.Wgs84Lng, out var lng) ? lng : null,
                    Latitude = decimal.TryParse(record.Wgs84Lat, out var lat) ? lat : null
                };
                newSites.Add(site);
                existingSites[record.SiteId] = site;
            }
            else
            {
                // 更新現有場址信息
                site.SiteName = record.SiteName ?? site.SiteName;
                site.County = record.County ?? site.County;
                site.Township = record.Township ?? site.Township;
                site.Address = record.Pollutantaddress ?? site.Address;
                site.SiteType = record.SiteType ?? site.SiteType;
                site.SiteUse = record.SiteUse ?? site.SiteUse;
                site.Pollutants = record.Pollutant ?? site.Pollutants;
                site.ControlType = record.Controltype ?? site.ControlType;
                site.SiteArea = decimal.TryParse(record.Sitearea, out var area) ? area : site.SiteArea;
                site.LandLots = record.Landno ?? site.LandLots;
                site.Dtmx = decimal.TryParse(record.Dtmx, out var dtmx) ? dtmx : site.Dtmx;
                site.Dtmy = decimal.TryParse(record.Dtmy, out var dtmy) ? dtmy : site.Dtmy;
                site.Longitude = decimal.TryParse(record.Wgs84Lng, out var lng) ? lng : site.Longitude;
                site.Latitude = decimal.TryParse(record.Wgs84Lat, out var lat) ? lat : site.Latitude;
            }
        }

        _logger.LogInformation("批次處理完成 - 有效記錄: {ValidRecords}, 無效記錄: {InvalidRecords}", 
            validRecords, invalidRecords);

        if (newSites.Any())
        {
            dbContext.PollutionSites.AddRange(newSites);
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 同步公告數據，避免N+1查詢問題
    /// </summary>
    private async Task<int> SyncAnnouncementsAsync()
    {
        _logger.LogInformation("開始同步公告數據");

        // 獲取總數
        var firstResponse = await _moenvApiService.GetEmsS08DataAsync(0, 1);
        if (firstResponse?.Records == null || !firstResponse.Records.Any())
        {
            _logger.LogWarning("沒有獲取到任何公告數據");
            return 0;
        }

        var totalRecords = int.TryParse(firstResponse.Total, out var total) ? total : 0;
        if (totalRecords == 0) return 0;

        _logger.LogInformation("總共需要處理 {Total} 條公告記錄", totalRecords);

        const int batchSize = 1000;
        var processedCount = 0;

        // 預先加載所有場址ID映射，避免N+1查詢
        var siteIdMap = await _dbContext.PollutionSites
            .Select(s => new { s.Id, s.SiteId })
            .ToDictionaryAsync(x => x.SiteId, x => x.Id);

        for (int offset = 0; offset < totalRecords; offset += batchSize)
        {
            _logger.LogInformation("正在處理第 {Offset} 到 {End} 條公告記錄", 
                offset, Math.Min(offset + batchSize, totalRecords));

            var response = await _moenvApiService.GetEmsS08DataAsync(offset, batchSize);
            if (response?.Records == null || !response.Records.Any()) continue;

            var records = response.Records.ToList();
            var announcementNos = records.Select(r => r.Annono).Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList();
            
            // 批量查詢已存在的公告
            var existingAnnouncements = await _dbContext.SiteAnnouncements
                .Where(a => announcementNos.Contains(a.AnnouncementNo))
                .ToDictionaryAsync(a => a.AnnouncementNo);

            var newAnnouncements = new List<SiteAnnouncement>();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.Annono) || string.IsNullOrEmpty(record.Siteid))
                    continue;

                // 檢查對應的場址是否存在
                if (!siteIdMap.TryGetValue(record.Siteid, out var pollutionSiteId))
                {
                    _logger.LogWarning("找不到對應的場址 {SiteId}，跳過公告 {AnnouncementNo}", 
                        record.Siteid, record.Annono);
                    continue;
                }

                if (!existingAnnouncements.TryGetValue(record.Annono, out var announcement))
                {
                    announcement = new SiteAnnouncement
                    {
                        Id = Guid.NewGuid(),
                        AnnouncementNo = record.Annono,
                        AnnouncementDate = DateTime.TryParse(record.Annodate, out var date) ? date : DateTime.MinValue,
                        Title = record.Annotitle,
                        Content = record.Annocontent,
                        IsSoilPollutionZone = record.Issoil == "1",
                        IsGroundwaterPollutionZone = record.Isgw == "1",
                        PollutionSiteId = pollutionSiteId
                    };
                    newAnnouncements.Add(announcement);
                    existingAnnouncements[record.Annono] = announcement;
                }
                else
                {
                    // 更新現有公告信息
                    announcement.AnnouncementDate = DateTime.TryParse(record.Annodate, out var date) ? date : announcement.AnnouncementDate;
                    announcement.Title = record.Annotitle ?? announcement.Title;
                    announcement.Content = record.Annocontent ?? announcement.Content;
                    announcement.IsSoilPollutionZone = record.Issoil == "1";
                    announcement.IsGroundwaterPollutionZone = record.Isgw == "1";
                    announcement.PollutionSiteId = pollutionSiteId;
                }
            }

            if (newAnnouncements.Any())
            {
                _dbContext.SiteAnnouncements.AddRange(newAnnouncements);
            }

            await _dbContext.SaveChangesAsync();
            processedCount += records.Count;
        }

        return processedCount;
    }

    private void PrintErrorRecord<T>(T record)
    {
        _logger.LogWarning("數據: {Serialize}", JsonSerializer.Serialize(record, new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) 
        }));
    }
}