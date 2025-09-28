namespace RiverMonitor.Model.Entities;

public class IrrigationAgencyStationMonitoringData
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 名稱
    /// </summary>
    public required string SiteName { get; set; }
    
    /// <summary>
    /// 採樣日期與時間
    /// </summary>
    public DateTime SampleDate { get; set; }
    
    /// <summary>
    /// 水溫_C
    /// </summary>
    public decimal? WaterTemperatureC { get; set; }
    
    /// <summary>
    /// pH值
    /// </summary>
    public decimal? PhValue { get; set; }

    /// <summary>
    /// EC_μS_cm_
    /// </summary>
    public decimal? ElectricalConductivity { get; set; } 

    /// <summary>
    /// 備註
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Version
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// 外鍵：關聯的農田水利署工作站 ID
    /// </summary>
    public int IrrigationAgencyStationId { get; set; }

    /// <summary>
    /// 導覽屬性：關聯的農田水利署工作站
    /// </summary>
    public IrrigationAgencyStation Station { get; set; } = null!;
}