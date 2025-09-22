using RiverMonitor.Bll.ApiServices;

namespace RiverMonitor.Bll.Services;

public interface ISyncService
{
    Task SyncAsync();
}

public class SyncService : ISyncService
{
    private readonly IMoenvApiService _moenvApiService;

    public SyncService(IMoenvApiService moenvApiService)
    {
        _moenvApiService = moenvApiService;
    }

    public Task SyncAsync()
    {
        throw new NotImplementedException();
    }
}