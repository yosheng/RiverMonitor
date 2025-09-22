namespace RiverMonitor.Model.Entities;

/// <summary>
/// 代表一筆廢水排放許可證及其設施資訊。
/// </summary>
public class WastewaterPermit
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 管制編號 (ems_no)
    /// </summary>
    public string EmsNo { get; set; } = null!;

    /// <summary>
    /// 事業名稱 (fac_name)
    /// </summary>
    public string FacilityName { get; set; } = null!;

    /// <summary>
    /// 地址 (address)
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// 統一編號 (unino)
    /// </summary>
    public string? UniformNo { get; set; }

    /// <summary>
    /// 許可證字號 (per_no)
    /// </summary>
    public string? PermitNo { get; set; }

    /// <summary>
    /// 許可證核准日期 (per_sdate)
    /// </summary>
    public DateTime? PermitStartDate { get; set; }

    /// <summary>
    /// 許可證有效截止日期 (per_edate)
    /// </summary>
    public DateTime? PermitEndDate { get; set; }

    /// <summary>
    /// 許可類別 (per_type)
    /// </summary>
    public string? PermitType { get; set; }
    
    /// <summary>
    /// 核准排放量 (per_water)
    /// </summary>
    public decimal? PermittedWaterVolume { get; set; }

    // --- 放流口資訊 (let_*) ---
    
    /// <summary>
    /// 放流口編號 (let)
    /// </summary>
    public string? OutletId { get; set; }
    
    /// <summary>
    /// 放流口 TWD97 X座標 (let_tm2x)
    /// </summary>
    public decimal? OutletTm2x { get; set; }

    /// <summary>
    /// 放流口 TWD97 Y座標 (let_tm2y)
    /// </summary>
    public decimal? OutletTm2y { get; set; }

    /// <summary>
    /// 放流口 WGS84 經度 (let_east)
    /// </summary>
    public decimal? OutletLongitude { get; set; }
    
    /// <summary>
    /// 放流口 WGS84 緯度 (let_north)
    /// </summary>
    public decimal? OutletLatitude { get; set; }
    
    /// <summary>
    /// 承受水體 (let_watertype)
    /// </summary>
    public string? OutletWaterType { get; set; }

    /// <summary>
    /// 導覽屬性：此許可證下的所有污染物排放紀錄
    /// </summary>
    public ICollection<PollutantEmission> Emissions { get; set; } = new List<PollutantEmission>();
}