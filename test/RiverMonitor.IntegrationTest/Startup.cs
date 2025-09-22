using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using RiverMonitor.Bll;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Dal;
using SynyiWorkManager.Provider;
using Xunit.DependencyInjection.Logging;

namespace RiverMonitor.IntegrationTest;

public class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder
            .ConfigureAppConfiguration(builder =>
            {
                // 注册配置
                builder
                    .AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        {
                            "ConnectionString",
                            "Data Source=../RiverMonitor.Dal/MyDatabase.db;"
                        }
                    }!)
                    .AddJsonFile("appsettings.json")
                    .Add(new SystemSettingConfigurationSource(
                        "Data Source=MyDatabase.db;"));
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(lb => lb.AddXunitOutput());
                
                services.AddDbContext<RiverMonitorDbContext>(
                    options => options.UseSqlite(context.Configuration["ConnectionString"])
                );

                services.AddServices();
                services.AddTransient(sp => new ApiKeyHandler(context.Configuration["Endpoint:MoenvApiKey"]!));

                services.AddRefitClient<IMoenvApiService>(new RefitSettings
                    {
                        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                            PropertyNameCaseInsensitive = true
                        })
                    })
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(context.Configuration["Endpoint:MoenvApi"]!))
                    .AddHttpMessageHandler<ApiKeyHandler>();


                services.AddAutoMapper(cfg =>
                {
                });
            });
    }
}