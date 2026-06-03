using Acme.BookStore.Web.Data;

using Amazon.S3;

using OpenTelemetry.Exporter;

using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.OpenTelemetry;

namespace Acme.BookStore.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);
        var loggerConfiguration = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                // succcess
                .WriteTo.Async(c => c.File("Logs/logs.log"))
                .WriteTo.Async(c => c.Console())
                .WriteTo.Async(c => c.File(new JsonFormatter(), path: "Logs/logs.log")) // success
                .WriteTo.Async(c => c.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri("http://localhost:4080/api/_bulk"))
                    {
                        // ZincSearch usa el endpoint /api/_bulk para compatibilidad
                        AutoRegisterTemplate = true,
                        IndexFormat = "apps-logs-{0:yyyy.MM.dd}", // Nombre del índice en ZincSearch
                        ModifyConnectionSettings = x => x.BasicAuthentication("user", "user"),
                        TypeName = null,
                        BatchPostingLimit = 50,
                    }))
                // .WriteTo.Async(a => a.AmazonS3())
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
                    opts.Endpoint = "http://localhost:3318/v1/logs";
                    opts.Protocol = OtlpProtocol.Grpc;
                }))

                // not works
                .WriteTo.GrafanaLoki(
                    "http://localhost:3100",
                    labels: new[]
                    {
                        new LokiLabel { Key = "app", Value = "mi-api" },
                        new LokiLabel { Key = "env", Value = "prod" }
                    }
                    // ,
                    // credentials: new LokiCredentials()
                    // {
                    //     Login = "",
                    //     Password = "",
                    // }
                )
                .WriteTo.Async(c => c.Async(a => a.OpenObserve(
                    "http://localhost:5080/api/default",
                    "default",
                    "root@example.com",
                    "Complexpass#123"
                )))
            // TODO: se tiene que configurar bien el fluent-bit el input http y el output file
            // .WriteTo.Async(c => c.DurableHttpUsingFileSizeRolledBuffers("http://localhost:8888/",
            //     period: TimeSpan.FromSeconds(2),
            //     textFormatter: new Serilog.Formatting.Json.JsonFormatter()))
            ;
        // https://openobserve.ai/blog/serilog-sink-for-openobserve/
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
