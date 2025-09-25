using RiverMonitor.Bll.ApiServices;

namespace RiverMonitor.Bll.IntegrationTest.ApiServices;

public class MoenvApiServiceTest
{
    private readonly IMoenvApiService _moenvApiService;
    private readonly ITestOutputHelper _output;

    public MoenvApiServiceTest(IMoenvApiService moenvApiService, ITestOutputHelper output)
    {
        _moenvApiService = moenvApiService;
        _output = output;
    }
    
    [Fact]
    public async Task GetEmsS03DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsS03DataAsync(0, 5);
        _output.WriteLine(result.Total!);
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetEmsS07DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsS07DataAsync(0, 5);
        _output.WriteLine(result.Total!);
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetEmsS08DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetEmsS08DataAsync(0, 5);
        _output.WriteLine(result.Total!);
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task GetWqxP01DataAsync_ShouldReturnData()
    {
        var result = await _moenvApiService.GetWqxP01DataAsync(0, 5);
        Assert.NotNull(result);
    }
}