using Acme.BookStore.Web.Data;

using Amazon.Runtime;

using AWS.Logger;
using AWS.Logger.SeriLog;

using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;

namespace Acme.BookStore.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.log"))
            .WriteTo.Async(c => c.Console())
            // TODO: se tiene que configurar bien el fluent-bit el input http y el output file
            .WriteTo.Async(c => c.DurableHttpUsingFileSizeRolledBuffers("http://localhost:8888/",
                period: TimeSpan.FromSeconds(2),
                textFormatter: new Serilog.Formatting.Json.JsonFormatter()))
            // .WriteTo.Async(c => c.AWSSeriLog(new AWSLoggerConfig()
            // {
            //     Region = "us-east-2",
            //     LogGroup = "AppAspnetCoreLogs",
            //     LogStreamNamePrefix = "api_",
            //     BatchPushInterval = TimeSpan.FromSeconds(5),
            //     Credentials = new BasicAWSCredentials("", ""),
            //     LogStreamName = "api-2026",
            // }, textFormatter: new Serilog.Formatting.Json.JsonFormatter()))
            .WriteTo.Async(c => c.OpenTelemetry(opts =>
            {
                opts.ResourceAttributes = new Dictionary<string, object>
                {
                    ["app"] = "webapi", ["runtime"] = "dotnet", ["service.name"] = "WebApi"
                };
            }));

        if (IsMigrateDatabase(args))
        {
            loggerConfiguration.MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning);
            loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog()
                ;
            if (IsMigrateDatabase(args))
            {
                // builder.Services.AddDataMigrationEnvironment();
            }

            await builder.AddApplicationAsync<WebModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();

            if (IsMigrateDatabase(args))
            {
                await app.Services.GetRequiredService<WebDbMigrationService>().MigrateAsync();
                return 0;
            }

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Acme.BookStore.Web terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static bool IsMigrateDatabase(string[] args)
    {
        return args.Any(x => x.Contains("--migrate-database", StringComparison.OrdinalIgnoreCase));
    }
}
