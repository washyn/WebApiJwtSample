using Microsoft.Extensions.DependencyInjection;
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
        context.Services.AddTransient<ReadmeEtlRunner>();
        context.Services.AddLogging();
        context.Services.AddHostedService<AppHostedService>();
    }
}

public class AppHostedService : IHostedService
{
    private readonly ReadmeEtlRunner _runner;
    private readonly ILogger<AppHostedService> _logger;

    public AppHostedService(ReadmeEtlRunner runner, ILogger<AppHostedService> logger)
    {
        _runner = runner;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _runner.RunAsync();
        _logger.LogInformation("EtlDapper started");
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

        using var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
            .UseSerilog()
            .ConfigureServices((hostContext, services) => { services.AddApplication<EtlDapperModule>(); });
}
