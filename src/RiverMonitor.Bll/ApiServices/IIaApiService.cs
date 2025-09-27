using Refit;

namespace RiverMonitor.Bll.ApiServices;

public interface IIaApiService
{
    [Get("/zh-TW/about/officeList")]
    Task<string> GetOfficeListPageAsync([Query("a")] int agencyId = 93);
}