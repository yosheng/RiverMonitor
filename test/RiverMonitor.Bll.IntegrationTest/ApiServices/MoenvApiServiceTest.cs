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
    public async Task GetEmsS03DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsS03DataAsync(0, 5);
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetEmsS07DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsS07DataAsync(0, 5);
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetEmsS08DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsS08DataAsync(0, 5);
        Assert.NotNull(result);
    }
}