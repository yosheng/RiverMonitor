using Microsoft.EntityFrameworkCore;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RiverMonitorDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}