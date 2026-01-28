using EtlDapper.Lib;
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
        context.Services.AddLogging();
        context.Services.AddHostedService<AppHostedService>();
    }
}

public class AppHostedService : IHostedService
{
    private readonly ILogger<AppHostedService> _logger;
    private readonly DatitoSource _source;
    private readonly IdentityTransform<PeopleRecord> _transform;
    private readonly PeoplesDestination _destination;

    public AppHostedService(ILogger<AppHostedService> logger,
        DatitoSource source, IdentityTransform<PeopleRecord> transform, PeoplesDestination destination)
    {
        _logger = logger;
        _source = source;
        _transform = transform;
        _destination = destination;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var pipeline = new EtlPipeline<PeopleRecord, PeopleRecord>(_source, _transform, _destination, 10_000);
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

        using var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
            .UseSerilog()
            .ConfigureServices((hostContext, services) => { services.AddApplication<EtlDapperModule>(); });
}
