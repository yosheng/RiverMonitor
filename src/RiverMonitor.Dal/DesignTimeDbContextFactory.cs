using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RiverMonitor.Dal;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RiverMonitorDbContext>
{
    public RiverMonitorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RiverMonitorDbContext>();
        optionsBuilder
            .UseSqlServer("Server=127.0.0.1,1401;Database=MyDatabase;User Id=sa;Password=StrongP@ssw0rd!;Trusted_Connection=False;TrustServerCertificate=true");

        return new RiverMonitorDbContext(optionsBuilder.Options);
    }
}