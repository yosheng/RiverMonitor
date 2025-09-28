using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Dal;

public class RiverMonitorDbContext : DbContext
{
    public RiverMonitorDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<WastewaterPermit> WastewaterPermits { get; set; }
    public DbSet<PollutantEmission> PollutantEmissions { get; set; }
    public DbSet<PollutionSite> PollutionSites { get; set; }
    public DbSet<SiteAnnouncement> SiteAnnouncements { get; set; }
    public DbSet<MonitoringSite> MonitoringSites { get; set; }
    public DbSet<MonitoringSiteSample> MonitoringSiteSamples { get; set; }
    public DbSet<GroundwaterSite> GroundwaterSites { get; set; }
    public DbSet<GroundwaterSiteSample> GroundwaterSiteSamples { get; set; }
    public DbSet<IrrigationAgency> IrrigationAgencies { get; set; }
    public DbSet<IrrigationAgencyStation> IrrigationAgencyStations { get; set; }
    public DbSet<IrrigationAgencyStationMonitoringData> IrrigationAgencyStationMonitoringData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RiverMonitorDbContext).Assembly);

        modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting { Id = 3, Key = "Endpoint:MoaApi", Value = "https://data.moa.gov.tw" },
            new SystemSetting { Id = 4, Key = "Endpoint:IaApi", Value = "https://www.ia.gov.tw" }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}