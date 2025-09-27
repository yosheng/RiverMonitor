namespace RiverMonitor.Model.Entities;

/// <summary>
/// 農田水利署
/// </summary>
public class IrrigationAgency
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 開放資料集的單位代碼
    /// </summary>
    public required string OpenUnitId { get; set; }

    /// <summary>
    /// 工作站網址
    /// </summary>
    public string? WorkStationUrl { get; set; }
    
    /// <summary>
    /// 導覽屬性：此農田水利署下的所有工作站
    /// </summary>
    public ICollection<IrrigationAgencyStation> Stations { get; set; } = new List<IrrigationAgencyStation>();
}