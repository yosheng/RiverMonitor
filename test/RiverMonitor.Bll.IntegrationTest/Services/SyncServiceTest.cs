using RiverMonitor.Bll.Services;

namespace RiverMonitor.Bll.IntegrationTest.Services;

public class SyncServiceTest
{
    private readonly ISyncService _syncService;

    public SyncServiceTest(ISyncService syncService)
    {
        _syncService = syncService;
    }

    [Fact]
    public async Task SyncWastewaterEmissionAsync()
    {
        await _syncService.SyncWastewaterEmissionAsync();

        Assert.True(true);
    }

    [Fact]
    public async Task SyncPollutionSiteAndAnnouncementAsync()
    {
        await _syncService.SyncPollutionSiteAndAnnouncementAsync();

        Assert.True(true);
    }
}