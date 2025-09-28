using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.IntegrationTest.ApiServices;

public class MoaApiServiceTest
{
    private IMoaApiService _moaApiService;

    public MoaApiServiceTest(IMoaApiService moaApiService)
    {
        _moaApiService = moaApiService;
    }

    [Fact]
    public async Task GetOpenThematicAsync_ShouldReturnData()
    {
        var response = await _moaApiService.GetOpenThematicAsync(new ThematicRequestForm
        {
            Page = 1,
            Limit = 1000
        });

        var data = response.Data.Where(x => x.Title != null && x.Title.Contains("水質監測結果")).ToList();
        
        Assert.NotEmpty(data);
    }
}