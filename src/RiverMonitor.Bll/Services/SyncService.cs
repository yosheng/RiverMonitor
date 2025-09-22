using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Dal;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Bll.Services;

public interface ISyncService
{
    Task SyncWastewaterEmissionAsync();
}

public class SyncService : ISyncService
{
    private readonly IMoenvApiService _moenvApiService;
    private readonly RiverMonitorDbContext _dbContext;
    private readonly ILogger<SyncService> _logger;

    public SyncService(
        IMoenvApiService moenvApiService, 
        RiverMonitorDbContext dbContext,
        ILogger<SyncService> logger)
    {
        _moenvApiService = moenvApiService;
        _dbContext = dbContext;
        _logger = logger;
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

        const int batchSize = 1000;
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
}