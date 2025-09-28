using Microsoft.AspNetCore.Mvc;
using RiverMonitor.Bll.Services;

namespace RiverMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// 同步水質排放資料
    /// </summary>
    [HttpPost("wastewater-emission")]
    public async Task<string> SyncWastewaterEmissionAsync()
    {
        await _syncService.SyncWastewaterEmissionAsync();
        return "数据同步成功";
    }
    
    /// <summary>
    /// 同步水質站点及公告資料
    /// </summary>
    [HttpPost("pollution-site-and-announcement")]
    public async Task<string> SyncPollutionSiteAndAnnouncementAsync()
    {
        await _syncService.SyncPollutionSiteAndAnnouncementAsync();
        return "数据同步成功";
    }
}