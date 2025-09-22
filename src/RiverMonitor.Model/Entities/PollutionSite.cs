namespace RiverMonitor.Model.Entities;

/// <summary>
/// 代表一個受污染的場址。
/// </summary>
public class PollutionSite
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 場址代碼 (site_id) - 業務上的唯一識別碼
    /// </summary>
    public string SiteId { get; set; } = null!;

    /// <summary>
    /// 場址名稱 (site_name)
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    /// 縣市 (county)
    /// </summary>
    public string? County { get; set; }

    /// <summary>
    /// 鄉鎮市區 (township)
    /// </summary>
    public string? Township { get; set; }

    /// <summary>
    /// 場址地址 (pollutantaddress)
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 場址類型 (site_type)
    /// </summary>
    public string? SiteType { get; set; }

    /// <summary>
    /// 場址使用狀態 (site_use)
    /// </summary>
    public string? SiteUse { get; set; }
    
    /// <summary>
    /// 污染物描述 (pollutant)
    /// </summary>
    public string? Pollutants { get; set; }
    
    /// <summary>
    /// 場址管制類型 (controltype)
    /// </summary>
    public string? ControlType { get; set; }

    /// <summary>
    /// 場址面積，單位：平方公尺 (sitearea)
    /// </summary>
    public decimal? SiteArea { get; set; }

    /// <summary>
    /// 地段地號 (landno)
    /// </summary>
    public string? LandLots { get; set; }
    
    // --- 座標資訊 ---
    
    /// <summary>
    /// TWD97 X座標 (dtmx)
    /// </summary>
    public decimal? Dtmx { get; set; }

    /// <summary>
    /// TWD97 Y座標 (dtmy)
    /// </summary>
    public decimal? Dtmy { get; set; }
    
    /// <summary>
    /// WGS84 經度 (wgs84_lng)
    /// </summary>
    public decimal? Longitude { get; set; }
    
    /// <summary>
    /// WGS84 緯度 (wgs84_lat)
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// 導覽屬性：此場址的所有相關公告
    /// </summary>
    public ICollection<SiteAnnouncement> Announcements { get; set; } = new List<SiteAnnouncement>();
}