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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncDataAsync()
    {
        try
        {
            await _syncService.SyncAsync();
            return Ok("数据同步成功");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"同步失败: {ex.Message}");
        }
    }
}