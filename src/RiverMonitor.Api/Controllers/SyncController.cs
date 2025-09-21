using RiverMonitor.Bll.Services;

namespace RiverMonitor.Api.Controllers;

public class SyncController
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }
}