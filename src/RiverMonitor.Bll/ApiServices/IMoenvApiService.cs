using Refit;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.ApiServices;

public interface IMoenvApiService
{
    /// <summary>
    /// 水污染源許可及申報資料
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [Get("/ems_s_03")]
    Task<EmsS03Data.RootObject> GetEmsS03DataAsync(int offset = 0, int limit = 10);    
    
    /// <summary>
    /// 土壤及地下水污染場址基本資料
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [Get("/ems_s_07")]
    Task<EmsS07Data.RootObject> GetEmsS07DataAsync(int offset = 0, int limit = 10);    
    
    /// <summary>
    /// 土壤及地下水污染管制區公告資料
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [Get("/ems_s_08")]
    Task<EmsS08Data.RootObject> GetEmsS08DataAsync(int offset = 0, int limit = 10);
}