namespace RiverMonitor.Bll.Services;

public interface ISyncService
{
    Task SyncAsync();
}

public class SyncService : ISyncService
{
    public Task SyncAsync()
    {
        throw new NotImplementedException();
    }
}