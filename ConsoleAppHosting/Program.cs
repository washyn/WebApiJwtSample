using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder();
builder.Services.AddHostedService<TimeService>();
builder.Services.AddHostedService<TimeService2>();
builder.Services.Configure<HostOptions>(a =>
{
    a.StartupTimeout = TimeSpan.FromSeconds(5);
    a.ServicesStartConcurrently = true;
});
IHost host = builder.Build();

var lifeTime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifeTime.ApplicationStarted.Register(() => Console.WriteLine("hiii"));
await host.RunAsync();





public class TimeService : IHostedService
{
    private readonly ILogger<TimeService> _logger;

    public TimeService(ILogger<TimeService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogInformation("The time eis {Time}", TimeOnly.FromDateTime(DateTime.Now));
                await Task.Delay(2000, cancellationToken);
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}


public class TimeService2 : IHostedService
{
    private readonly ILogger<TimeService> _logger;

    public TimeService2(ILogger<TimeService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogInformation("The time eis {Time}", TimeOnly.FromDateTime(DateTime.Now));
                await Task.Delay(2000, cancellationToken);
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}