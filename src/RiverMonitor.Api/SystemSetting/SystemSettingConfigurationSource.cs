namespace RiverMonitor.Api.SystemSetting;

public sealed class SystemSettingConfigurationSource : IConfigurationSource
{
    private readonly string? _connectionString;

    public SystemSettingConfigurationSource(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new SystemSettingConfigurationProvider(_connectionString);
}