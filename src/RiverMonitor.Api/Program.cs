using Microsoft.EntityFrameworkCore;
using Refit;
using RiverMonitor.Api;
using RiverMonitor.Bll;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Dal;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemSettingConfiguration();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<RiverMonitorDbContext>(
    options => options.UseSqlite(builder.Configuration["ConnectionString"])
);

builder.Services.AddServices();
builder.Services.AddTransient(sp => new ApiKeyHandler(builder.Configuration["Endpoint:MoenvApiKey"]!));

builder.Services.AddRefitClient<IMoenvApiService>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["Endpoint:MoenvApi"]!))
    .AddHttpMessageHandler<ApiKeyHandler>();

builder.Services.AddAutoMapper(cfg =>
{
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();