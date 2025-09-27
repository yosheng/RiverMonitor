namespace RiverMonitor.Api.SystemSetting;

public sealed class SystemSettingConfigurationProvider : ConfigurationProvider
{
    private readonly string? _connectionString;

    public SystemSettingConfigurationProvider(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public override void Load()
    {
        using var dbContext = new SystemSettingConfigurationContext(_connectionString);

        dbContext.Database.EnsureCreated();

        Data = dbContext.SystemSetting.Any()
            ? dbContext.SystemSetting.ToDictionary(
                static c => c.Key,
                static c => c.Value)
            : CreateAndSaveDefaultValues(dbContext);
    }
    
    static Dictionary<string, string?> CreateAndSaveDefaultValues(
        SystemSettingConfigurationContext context)
    {
        var settings = new Dictionary<string, string?>
        {
            ["Endpoint:MoenvApi"] = "https://data.moenv.gov.tw/api/v2",
            ["Endpoint:MoenvApiKey"] = "5d235b2e-ba39-4dab-8b03-25bbf93d437e",
        };

        var data = (from setting in settings
            select new Model.Entities.SystemSetting()
            {
                Key = setting.Key,
                Value = setting.Value
            }).ToList();
        
        context.SystemSetting.AddRange(data);

        context.SaveChanges();

        return settings;
    }
}