using Microsoft.EntityFrameworkCore;

namespace RiverMonitor.Dal;

public class RiverMonitorDbContext : DbContext
{
    public RiverMonitorDbContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}