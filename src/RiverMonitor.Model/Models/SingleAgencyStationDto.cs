namespace RiverMonitor.Model.Models;

public record SingleAgencyStationDto
{
    public required string AgencyName { get; set; }

    public string? OpenUnitId { get; set; }

    public int Id { get; set; }
    
    public required string Name { get; set; }
}