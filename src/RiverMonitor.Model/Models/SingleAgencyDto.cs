namespace RiverMonitor.Model.Models;

public record SingleAgencyDto
{
    public required string AgencyName { get; set; }

    public string? OpenUnitId { get; set; }
}