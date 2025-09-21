using SynyiWorkManager.Provider;

namespace RiverMonitor.Api;

public static class SystemSettingManagerExtensions
{
    public static ConfigurationManager AddSystemSettingConfiguration(
        this ConfigurationManager manager)
    {
        var connectionString = manager.GetValue<string>("ConnectionString");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("未配置系统配置链接字符串");
        }
        
        IConfigurationBuilder configBuilder = manager;
        configBuilder.Add(new SystemSettingConfigurationSource(connectionString));

        return manager;
    }
}