using Microsoft.EntityFrameworkCore;

namespace RiverMonitor.Api.SystemSetting;

public sealed class SystemSettingConfigurationContext : DbContext
{
    private readonly string? _connectionString;

    public SystemSettingConfigurationContext(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Model.Entities.SystemSetting> SystemSettings => Set<Model.Entities.SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SystemSettingConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connectionString)
            .UseSnakeCaseNamingConvention();
    }
}