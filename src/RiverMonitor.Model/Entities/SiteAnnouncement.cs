namespace RiverMonitor.Model.Entities;

/// <summary>
/// 代表一則污染場址的相關公告。
/// </summary>
public class SiteAnnouncement
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 公告文號 (annono)
    /// </summary>
    public string AnnouncementNo { get; set; } = null!;

    /// <summary>
    /// 公告日期 (annodate)
    /// </summary>
    public DateTime AnnouncementDate { get; set; }

    /// <summary>
    /// 公告主旨 (annotitle)
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 公告內容 (annocontent)
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 是否涉及土壤污染管制區 (issoil, 1=是, 0=否)
    /// </summary>
    public bool IsSoilPollutionZone { get; set; }

    /// <summary>
    /// 是否涉及地下水污染管制區 (isgw, 1=是, 0=否)
    /// </summary>
    public bool IsGroundwaterPollutionZone { get; set; }
    
    /// <summary>
    /// 外鍵：關聯的污染場址 ID
    /// </summary>
    public Guid PollutionSiteId { get; set; }

    /// <summary>
    /// 導覽屬性：關聯的污染場址
    /// </summary>
    public PollutionSite Site { get; set; } = null!;
}