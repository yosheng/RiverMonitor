namespace RiverMonitor.Model.Entities;

/// <summary>
/// 農田水利署工作站
/// </summary>
public class IrrigationAgencyStation
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
    /// TWD97經度
    /// </summary>
    public decimal? Twd97Lon { get; set; }

    /// <summary>
    /// TWD97緯度
    /// </summary>
    public decimal? Twd97Lat { get; set; }

    /// <summary>
    /// 電話
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// 外鍵：關聯的農田水利署 ID
    /// </summary>
    public Guid IrrigationAgencyId { get; set; }

    /// <summary>
    /// 導覽屬性：關聯的農田水利署
    /// </summary>
    public IrrigationAgency Agency { get; set; } = null!;
}