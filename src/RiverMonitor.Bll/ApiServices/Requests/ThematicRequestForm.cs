using Refit;

namespace RiverMonitor.Model.ApiModels;

/// <summary>
/// 代表發送到 open_list_thematic.ashx 的表單請求資料。
/// </summary>
public class ThematicRequestForm
{
    [AliasAs("Page")]
    public int Page { get; set; }

    [AliasAs("Limit")]
    public int Limit { get; set; }

    [AliasAs("Keyword")]
    public string? Keyword { get; set; }

    [AliasAs("Sel_Catalog")]
    public string? Sel_Catalog { get; set; }

    [AliasAs("Sel_Organ")]
    public string? Sel_Organ { get; set; }

    [AliasAs("Sel_Format")]
    public string? Sel_Format { get; set; }

    [AliasAs("Ord")]
    public int Ord { get; set; }

    // C# 屬性使用 PascalCase, 透過 AliasAs 對應到 API 需要的小寫開頭欄位
    [AliasAs("sel_CatalogSecond")]
    public string? SelCatalogSecond { get; set; }

    [AliasAs("sel_CatalogThird")]
    public string? SelCatalogThird { get; set; }
}