using Flurl.Http;
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

    [Fact]
    public async Task SyncMonitoringSiteAsync()
    {
        await _syncService.SyncMonitoringSiteAsync();

        Assert.True(true);
    }

    [Fact]
    public async Task SyncGroundwaterSiteAsync()
    {
        await _syncService.SyncGroundwaterSiteAsync();

        Assert.True(true);
    }

    [Fact]
    public async Task SyncIrrigationAgencyAsync()
    {
        await _syncService.SyncIrrigationAgencyAsync();

        Assert.True(true);
    }

    [Fact]
    public async Task WorkStationUrlTest()
    {
        var result = await "https://www.iayli.nat.gov.tw/about/WorkStationPage?a=10411".GetStringAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task SyncIrrigationAgencyStationAsync()
    {
        await _syncService.SyncIrrigationAgencyStationAsync();
        
        Assert.True(true);
    }
}