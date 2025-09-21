using Refit;

namespace RiverMonitor.Bll.ApiServices;

public interface IMoenvApiService
{
    [Get("/api/v2/ems_s_03")]
    Task<string> GetEmsDataAsync();
}