using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using RiverMonitor.Api.SystemSetting;
using RiverMonitor.Bll;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Dal;
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
                            "Server=127.0.0.1,1401;Database=MyDatabase;User Id=sa;Password=StrongP@ssw0rd!;Trusted_Connection=False;TrustServerCertificate=true"
                        }
                    }!)
                    .AddJsonFile("appsettings.json")
                    .Add(new SystemSettingConfigurationSource(
                        "Server=127.0.0.1,1401;Database=MyDatabase;User Id=sa;Password=StrongP@ssw0rd!;Trusted_Connection=False;TrustServerCertificate=true"));
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(lb => lb.AddXunitOutput());
                
                services.AddDbContext<RiverMonitorDbContext>(
                    options => options.UseSqlServer(context.Configuration["ConnectionString"])
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