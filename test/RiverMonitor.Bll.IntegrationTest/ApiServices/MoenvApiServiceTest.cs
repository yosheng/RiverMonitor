using RiverMonitor.Bll.ApiServices;

namespace RiverMonitor.Bll.IntegrationTest.ApiServices;

public class MoenvApiServiceTest
{
    private readonly IMoenvApiService _moenvApiService;

    public MoenvApiServiceTest(IMoenvApiService moenvApiService)
    {
        _moenvApiService = moenvApiService;
    }
    
    [Fact]
    public async Task GetEmsDataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsDataAsync();
        Assert.NotNull(result);
    }
}