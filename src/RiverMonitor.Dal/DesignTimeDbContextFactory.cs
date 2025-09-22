using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RiverMonitor.Dal;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RiverMonitorDbContext>
{
    public RiverMonitorDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RiverMonitorDbContext>();
        optionsBuilder
            .UseSqlite("Data Source=MyDatabase.db;");

        return new RiverMonitorDbContext(optionsBuilder.Options);
    }
}