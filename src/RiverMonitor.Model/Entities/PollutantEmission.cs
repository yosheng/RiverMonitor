namespace RiverMonitor.Model.Entities;

public class PollutantEmission
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 申報起始日期 (emi_sdate)
    /// </summary>
    public DateTime? EmissionStartDate { get; set; }

    /// <summary>
    /// 申報結束日期 (emi_edate)
    /// </summary>
    public DateTime? EmissionEndDate { get; set; }

    /// <summary>
    /// 申報區間水量 (emi_water)
    /// </summary>
    public decimal? EmissionWaterVolume { get; set; }

    /// <summary>
    /// 申報區間水量單位 (emi_waterunit)
    /// </summary>
    public string? EmissionWaterUnit { get; set; }

    /// <summary>
    /// 檢測項目名稱 (emi_item)
    /// </summary>
    public string EmissionItemName { get; set; } = null!;

    /// <summary>
    /// 檢測濃度/數值 (emi_value)
    /// </summary>
    public decimal? EmissionValue { get; set; }

    /// <summary>
    /// 檢測濃度/數值單位 (emi_units)
    /// </summary>
    public string? EmissionUnit { get; set; }
    
    /// <summary>
    /// 排放量 (item_value)
    /// </summary>
    public decimal? TotalItemValue { get; set; }

    /// <summary>
    /// 排放量單位 (item_units)
    /// </summary>
    public string? TotalItemUnit { get; set; }

    /// <summary>
    /// 外鍵：關聯的廢水排放許可證 ID
    /// </summary>
    public Guid WastewaterPermitId { get; set; }

    /// <summary>
    /// 導覽屬性：關聯的廢水排放許可證
    /// </summary>
    public WastewaterPermit Permit { get; set; } = null!;
}