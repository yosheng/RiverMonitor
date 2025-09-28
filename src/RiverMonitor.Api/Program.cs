using System.Text.Json;
using FluentValidation;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.RecurringJobAdmin;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Refit;
using RiverMonitor.Api;
using RiverMonitor.Api.Filters;
using RiverMonitor.Api.Middleware;
using RiverMonitor.Bll;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Bll.Services;
using RiverMonitor.Dal;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemSettingConfiguration();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RiverMonitor API",
        Version = "v1",
        Description = "河流监测数据同步API",
        Contact = new OpenApiContact { Name = "RiverMonitor Team" }
    });

    Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml").ToList().ForEach(file =>
    {
        options.IncludeXmlComments(file, true);
    });
});

builder.Services.AddDbContext<RiverMonitorDbContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionString"])
);

// 配置 Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage()
    .UseRecurringJobAdmin(typeof(ServiceCollectionExtension).Assembly)
);

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount;
    options.Queues = new[] { "default", "sync" };
});

builder.Services.AddServices();
builder.Services.AddTransient(sp => new ApiKeyHandler(builder.Configuration["Endpoint:MoenvApiKey"]!));

builder.Services.AddRefitClient<IMoenvApiService>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        })
    })
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Endpoint:MoenvApi"]!))
    .AddHttpMessageHandler<ApiKeyHandler>();

builder.Services.AddRefitClient<IMoaApiService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Endpoint:MoaApi"]!));

builder.Services.AddRefitClient<IIaApiService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Endpoint:IaApi"]!));

builder.Services.AddAutoMapper(cfg => { });

// 自動掃描整個專案，找到所有繼承自 AbstractValidator 的類別並註冊到 DI 容器
builder.Services.AddValidatorsFromAssemblyContaining<ValidationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<RiverMonitorDbContext>();

        // 執行遷移
        dbContext.Database.Migrate();

        Console.WriteLine("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        // 處理遷移過程中可能發生的錯誤
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying database migrations.");
        // 在生產環境中，您可能需要更完善的錯誤處理機制
        // 例如：發送警報、重試或優雅地停止應用程式
    }
}

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(o => { o.RouteTemplate = $"/api/swagger/{{documentName}}/swagger.json"; });
    app.UseSwaggerUI(options => { options.SwaggerEndpoint($"/api/swagger/v1/swagger.json", "RiverMonitor API v1"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// 配置 Hangfire Dashboard 與密碼認證
var hangfireUsername = builder.Configuration["Hangfire:Username"] ?? "admin";
var hangfirePassword = builder.Configuration["Hangfire:Password"] ?? "admin123";

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorizationFilter(hangfireUsername, hangfirePassword) },
    DashboardTitle = "RiverMonitor Background Jobs",
    StatsPollingInterval = 2000,
    DisplayStorageConnectionString = false
});

app.MapControllers();

app.Run();