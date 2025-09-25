namespace RiverMonitor.Model.Entities;

/// <summary>
/// 代表在某個監測站的一次水質採樣紀錄。
/// </summary>
public class MonitoringSiteSample
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 採樣日期與時間
    /// </summary>
    public DateTime SampleDate { get; set; }

    public string? ItemName { get; set; }
    public string? ItemEngName { get; set; }
    public string? ItemEngAbbreviation { get; set; }
    
    /// <summary>
    /// 檢測值，使用 decimal 以確保數值精度
    /// </summary>
    public decimal? ItemValue { get; set; }
    
    public string? ItemUnit { get; set; }
    public string? Note { get; set; }

    /// <summary>
    /// 外鍵：關聯的監測站 ID
    /// </summary>
    public Guid MonitoringSiteId { get; set; }

    /// <summary>
    /// 導覽屬性：關聯的監測站
    /// </summary>
    public MonitoringSite Site { get; set; } = null!;
}