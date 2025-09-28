using Refit;
using RiverMonitor.Model.ApiModels;

namespace RiverMonitor.Bll.ApiServices;

public interface IMoaApiService
{
    /// <summary>
    /// 取得主題式開放資料列表。
    /// </summary>
    /// <param name="form">包含分頁、關鍵字等資訊的表單資料。</param>
    /// <returns>API 回應的資料。</returns>
    [Post("/api/open_list_thematic.ashx")]
    Task<OpenThematic.RootObject> GetOpenThematicAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        ThematicRequestForm form
    );
    
    [Post("/api/open_list.ashx")]
    Task<OpenThematic.RootObject> GetOpenDataAsync(
        [Body(BodySerializationMethod.UrlEncoded)]
        ThematicRequestForm form
    );

    /// <summary>
    /// 從主題式開放資料中取得水質資料。
    /// </summary>
    /// <returns>水質資料列表。</returns>
    [Get("/Service/OpenData/DataFileService.aspx")]
    Task<List<WaterQualityOpenData>> GetWaterQualityOpenDataAsync([Query("UnitId")] string unitId);
}