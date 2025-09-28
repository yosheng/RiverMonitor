using System.Text.Json.Serialization;

namespace RiverMonitor.Model.ApiModels;

public class WaterQualityOpenData
{
    [JsonPropertyName("管理處名稱")]
    public string? ManagementOfficeName { get; set; }

    [JsonPropertyName("工作站")]
    public string? Workstation { get; set; }

    [JsonPropertyName("名稱")]
    public string? SiteName { get; set; }

    [JsonPropertyName("採樣日期")]
    public string? SampleDate { get; set; }

    [JsonPropertyName("水溫°C")]
    public string? WaterTemperatureC { get; set; }

    [JsonPropertyName("pH值")]
    public string? PhValue { get; set; }

    [JsonPropertyName("EC(μS/cm)")]
    public string? ElectricalConductivity { get; set; } // EC (Electrical Conductivity)

    [JsonPropertyName("備註")]
    public string? Note { get; set; }

    [JsonPropertyName("Version")]
    public string? Version { get; set; }
}