using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace AwsSnsReceiver
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.log"
                    , rollingInterval: RollingInterval.Day
                    , retainedFileCountLimit: 2))
                .WriteTo.Async(c => c.Console())
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Host
                    .UseSerilog();

                await builder.AddApplicationAsync<WebModule>();
                var app = builder.Build();
                await app.InitializeApplicationAsync();

                Log.Information("Starting app...");
                await app.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    [DependsOn(typeof(AbpAspNetCoreMvcModule))]
    public class WebModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            // context.Services.AddHostedService<AppHostedService>();
            // context.Services.Configure<AwsSnsConfig>(a =>
            // {
            //     a.TopicArn = "arn:aws:sns:us-east-1:000000000000:mi-topic";
            //     a.AccessKeyId = "test";
            //     a.SecretAccessKey = "test";
            //     a.Region = "us-east-1";
            // });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            app.UseRouting();
            app.UseSerilogRequestLogging();
            app.UseConfiguredEndpoints();
        }
    }

    // public class AwsSnsConfig
    // {
    //     public string TopicArn { get; set; }
    //     public string Region { get; set; }
    //     public string AccessKeyId { get; set; }
    //     public string SecretAccessKey { get; set; }
    // }

    // public class AppHostedService : IHostedService
    // {
    //     private readonly ILogger<AppHostedService> _logger;
    //     private readonly AwsSnsConfig _options;

    //     public AppHostedService(ILogger<AppHostedService> logger, IOptions<AwsSnsConfig> options)
    //     {
    //         _logger = logger;
    //         _options = options.Value;
    //     }

    //     public async Task StartAsync(CancellationToken cancellationToken)
    //     {
    //         _logger.LogInformation($"Starting AppHostedService...");
    //     }

    //     public Task StopAsync(CancellationToken cancellationToken)
    //     {
    //         _logger.LogInformation($"Stopping AppHostedService...");
    //         return Task.CompletedTask;
    //     }
    // }
}

// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();
// app.MapPost("/sns", async (HttpRequest request) =>
// {
//     using var reader = new StreamReader(request.Body);
//     var body = await reader.ReadToEndAsync();
//     Console.WriteLine(body);
//     return Results.Ok();
// });
// app.Run();