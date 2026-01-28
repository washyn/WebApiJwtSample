using EtlDapper.Lib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Volo.Abp.Dapper;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace EtlDapper;

[DependsOn(typeof(AbpEntityFrameworkCoreSqliteModule))]
[DependsOn(typeof(AbpEntityFrameworkCorePostgreSqlModule))]
[DependsOn(typeof(AbpDapperModule))]
public class EtlDapperModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddTransient<DatitoSource>();
        context.Services.AddTransient<IdentityTransform<PeopleRecord>>();
        context.Services.AddTransient<PeoplesDestination>();
        context.Services.AddTransient<HealthCheckSqlite>();
        context.Services.AddLogging();
        context.Services.AddHostedService<AppHostedService>();
        context.Services.AddHealthChecks()
            .AddCheck<HealthCheckSqlite>("sqlite")
            .AddCheck<HealthCheckPostgres>("postgres");
    }
}

public class AppHostedService : IHostedService
{
    private readonly ILogger<AppHostedService> _logger;
    private readonly HealthCheckService _healthReport;
    private readonly DatitoSource _source;
    private readonly IdentityTransform<PeopleRecord> _transform;
    private readonly PeoplesDestination _destination;

    public AppHostedService(ILogger<AppHostedService> logger,
        HealthCheckService healthReport,
        DatitoSource source, IdentityTransform<PeopleRecord> transform, PeoplesDestination destination)
    {
        _logger = logger;
        _healthReport = healthReport;
        _source = source;
        _transform = transform;
        _destination = destination;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var healthReport = await _healthReport.CheckHealthAsync(cancellationToken);
        foreach (var entry in healthReport.Entries)
        {
            if (entry.Value.Status != HealthStatus.Healthy)
            {
                _logger.LogCritical("Health checks failed. Suspending ETL process.");
                return;
            }
        }

        var pipeline = new EtlPipeline<PeopleRecord, PeopleRecord>(_source, _transform, _destination, 10_000, _logger);
        await pipeline.RunAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Async(r => r.File("./logs/log.log", rollingInterval: RollingInterval.Day, shared: true,
                retainedFileCountLimit: 2))
            .WriteTo.Async(r => r.Console())
            .CreateLogger();

        await CreateHostBuilder(args).RunConsoleAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
            .UseSerilog()
            .ConfigureServices((hostContext, services) => { services.AddApplication<EtlDapperModule>(); });
}
