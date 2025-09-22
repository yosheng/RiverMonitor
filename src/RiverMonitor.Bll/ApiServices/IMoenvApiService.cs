using Refit;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.ApiServices;

public interface IMoenvApiService
{
    [Get("/ems_s_03")]
    Task<EmsS03Data.RootObject> GetEmsS03DataAsync(int offset = 0, int limit = 10);    
    
    [Get("/ems_s_07")]
    Task<EmsS07Data.RootObject> GetEmsS07DataAsync(int offset = 0, int limit = 10);    
    
    [Get("/ems_s_08")]
    Task<EmsS08Data.RootObject> GetEmsS08DataAsync(int offset = 0, int limit = 10);
}