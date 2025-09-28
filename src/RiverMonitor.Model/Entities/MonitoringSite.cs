namespace RiverMonitor.Model.Entities;

/// <summary>
/// 代表一個河川水質監測站。
/// </summary>
public class MonitoringSite
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 測站的業務編號 (來自外部資料源的 SiteId)
    /// </summary>
    public string SiteId { get; set; } = null!;

    public string? SiteName { get; set; }
    public string? SiteEngName { get; set; }
    public string? County { get; set; }
    public string? Township { get; set; }
    public string? Basin { get; set; }
    public string? River { get; set; }
    
    // 座標使用 decimal 以確保精度
    public decimal? Twd97Lon { get; set; }
    public decimal? Twd97Lat { get; set; }
    public decimal? Twd97Tm2X { get; set; }
    public decimal? Twd97Tm2Y { get; set; }
    
    public string? Statusofuse { get; set; }

    /// <summary>
    /// 導覽屬性：此測站下的所有水質樣本紀錄
    /// </summary>
    public ICollection<MonitoringSiteSample> Samples { get; set; } = new List<MonitoringSiteSample>();
}