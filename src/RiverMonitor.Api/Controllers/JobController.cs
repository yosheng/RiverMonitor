using Hangfire;
using Microsoft.AspNetCore.Mvc;
using RiverMonitor.Api.Services;
using RiverMonitor.Bll.Services;

namespace RiverMonitor.Api.Controllers;

/// <summary>
/// 背景任務控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly ISyncService _syncService;

    public JobController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// 觸發所有同步任務
    /// </summary>
    [HttpPost("trigger-all")]
    public IActionResult TriggerAllJobs()
    {
        BackgroundJobService.TriggerImmediateJobs();
        return Ok(new { message = "所有同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發廢水排放數據同步
    /// </summary>
    [HttpPost("trigger-wastewater-emission")]
    public IActionResult TriggerWastewaterEmissionSync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncWastewaterEmissionAsync());
        return Ok(new { jobId, message = "廢水排放數據同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發污染場址及公告數據同步
    /// </summary>
    [HttpPost("trigger-pollution-site-announcement")]
    public IActionResult TriggerPollutionSiteAnnouncementSync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncPollutionSiteAndAnnouncementAsync());
        return Ok(new { jobId, message = "污染場址及公告數據同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發河川水質監測數據同步
    /// </summary>
    [HttpPost("trigger-monitoring-site")]
    public IActionResult TriggerMonitoringSiteSync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncMonitoringSiteAsync());
        return Ok(new { jobId, message = "河川水質監測數據同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發地下水監測數據同步
    /// </summary>
    [HttpPost("trigger-groundwater-site")]
    public IActionResult TriggerGroundwaterSiteSync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncGroundwaterSiteAsync());
        return Ok(new { jobId, message = "地下水監測數據同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發農田水利署基本資料同步
    /// </summary>
    [HttpPost("trigger-irrigation-agency")]
    public IActionResult TriggerIrrigationAgencySync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncIrrigationAgencyAsync());
        return Ok(new { jobId, message = "農田水利署基本資料同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發農田水利署工作站資料同步
    /// </summary>
    [HttpPost("trigger-irrigation-agency-station")]
    public IActionResult TriggerIrrigationAgencyStationSync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncIrrigationAgencyStationAsync());
        return Ok(new { jobId, message = "農田水利署工作站資料同步任務已加入佇列" });
    }

    /// <summary>
    /// 觸發農田水利署監測數據同步
    /// </summary>
    [HttpPost("trigger-irrigation-agency-monitoring-data")]
    public IActionResult TriggerIrrigationAgencyMonitoringDataSync()
    {
        var jobId = BackgroundJob.Enqueue<ISyncService>(service => service.SyncIrrigationAgencyStationMonitoringDataAsync());
        return Ok(new { jobId, message = "農田水利署監測數據同步任務已加入佇列" });
    }

    /// <summary>
    /// 獲取所有定時任務狀態
    /// </summary>
    [HttpGet("recurring-jobs")]
    public IActionResult GetRecurringJobs()
    {
        var jobIds = new[]
        {
            "sync-wastewater-emission",
            "sync-pollution-site-announcement", 
            "sync-monitoring-site",
            "sync-groundwater-site",
            "sync-irrigation-agency",
            "sync-irrigation-agency-station",
            "sync-irrigation-agency-monitoring-data"
        };
        
        return Ok(new { 
            message = "定時任務列表", 
            jobs = jobIds.Select(id => new { jobId = id, status = "configured" }) 
        });
    }

    /// <summary>
    /// 刪除指定的定時任務
    /// </summary>
    [HttpDelete("recurring-jobs/{jobId}")]
    public IActionResult DeleteRecurringJob(string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
        return Ok(new { message = $"定時任務 {jobId} 已刪除" });
    }

    /// <summary>
    /// 立即執行指定的定時任務
    /// </summary>
    [HttpPost("recurring-jobs/{jobId}/trigger")]
    public IActionResult TriggerRecurringJob(string jobId)
    {
        RecurringJob.TriggerJob(jobId);
        return Ok(new { message = $"定時任務 {jobId} 已觸發執行" });
    }
}