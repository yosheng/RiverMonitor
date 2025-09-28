using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using RiverMonitor.Api.SystemSetting;
using RiverMonitor.Bll;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Bll.Services;
using RiverMonitor.Dal;
using Xunit.DependencyInjection.Logging;

namespace RiverMonitor.IntegrationTest;

public class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        DotNetEnv.Env.Load();

        var connectionString = DotNetEnv.Env.GetString("ConnectionString");
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString =
                "Server=127.0.0.1,1401;Database=MyDatabase;User Id=sa;Password=StrongP@ssw0rd!;Trusted_Connection=False;TrustServerCertificate=true";
        }

        hostBuilder
            .ConfigureAppConfiguration(builder =>
            {
                // 注册配置
                builder
                    .AddInMemoryCollection(new Dictionary<string, string>()
                    {
                        {
                            "ConnectionString",
                            connectionString
                        }
                    }!)
                    .AddJsonFile("appsettings.json")
                    .Add(new SystemSettingConfigurationSource(
                        connectionString));
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging(lb => lb.AddXunitOutput(options =>
                {
                    options.Filter = (category, logLevel) =>
                    {
                        if (category != null &&
                            category.Contains("Microsoft.EntityFrameworkCore.Database.Command"))
                        {
                            return logLevel >= LogLevel.Warning;
                        }

                        return true; // 其他 category 全都允許
                    };
                }));

                services.AddDbContext<RiverMonitorDbContext>(options =>
                    options.UseSqlServer(context.Configuration["ConnectionString"])
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
                
                
                services.AddRefitClient<IMoaApiService>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(context.Configuration["Endpoint:MoaApi"]!));

                services.AddRefitClient<IIaApiService>()
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(context.Configuration["Endpoint:IaApi"]!));
                
                services.AddAutoMapper(cfg => { });

                // 自動掃描整個專案，找到所有繼承自 AbstractValidator 的類別並註冊到 DI 容器
                services.AddValidatorsFromAssemblyContaining<ValidationService>();
            });
    }
}