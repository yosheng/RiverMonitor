using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Refit;
using RiverMonitor.Api;
using RiverMonitor.Api.Middleware;
using RiverMonitor.Bll;
using RiverMonitor.Bll.ApiServices;
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

builder.Services.AddDbContext<RiverMonitorDbContext>(
    options => options.UseSqlServer(builder.Configuration["ConnectionString"])
);

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

builder.Services.AddAutoMapper(cfg =>
{
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(o =>
    {
        o.RouteTemplate = $"/api/swagger/{{documentName}}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"/api/swagger/v1/swagger.json", "RiverMonitor API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();