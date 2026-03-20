using System.Diagnostics.Metrics;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using NewRelic.LogEnrichers.Serilog;

using Serilog;
using Serilog.Events;

using Volo.Abp.Modularity;

namespace WebAppChecker;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithNewRelicLogsInContext()
            .WriteTo.Async(a => a.Console())
            .WriteTo.Async(a => a.File("Logs/log.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7))
            .WriteTo.Async(c =>
                c.NewRelicLogs(applicationName: "AKDEMIC.HEALTHCHECK",
                    licenseKey: "edbce8c84a470c9198d9e049df18ede4FFFFNRAL"))
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        builder.Services.AddApplication<AppModule>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                Predicate = _ => true, ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        app.UseHealthChecksUI(options => options.UIPath = "/health-ui");
        app.UseAuthorization();
        app.MapRazorPages();

        app.Run();
    }
}

public class AppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddRazorPages();
        context.Services.AddHealthChecks()
            .AddCheck<LocalCheck>(nameof(LocalCheck))
            .AddCheck<TmiBaasAuthentication>(nameof(TmiBaasAuthentication));

        var config = context.Services.GetConfiguration();

        context.Services
            .AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(15);
                options.AddHealthCheckEndpoint("API", "/health");
            })
            .AddSqliteStorage(config.GetConnectionString("DefaultConnection") ??
                              throw new InvalidOperationException("DefaultConnection is not set."));
    }
}

public class LocalCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public LocalCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var response = await _httpClient.GetAsync("https://localhost:7259/healthz", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External server is healthy.");
            }
            else
            {
                return HealthCheckResult.Degraded(
                    $"External server responded with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error when check service:", ex);
            return HealthCheckResult.Unhealthy($"Error checking external server: {ex.Message}");
        }
    }
}

public class TmiBaasAuthentication : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public TmiBaasAuthentication(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:9001/health", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External server is healthy.");
            }
            else
            {
                return HealthCheckResult.Degraded(
                    $"External server responded with status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Log.Error("Error when check service:", ex);
            return HealthCheckResult.Unhealthy($"Error checking external server: {ex.Message}");
        }
    }
}

public static class MetricsDefinitions
{
    public static Meter Meter = new Meter("App.Metric", "1.0");

    public static Counter<long> RequestCounter =
        Meter.CreateCounter<long>("http_requests_total", description: "Numero total de peticiones http.");
}