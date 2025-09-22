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
    /// 同步河流监测数据
    /// </summary>
    /// <returns>同步结果</returns>
    [HttpPost("data")]
    public async Task<string> SyncDataAsync()
    {
        await _syncService.SyncWastewaterEmissionAsync();
        return "数据同步成功";
    }
}