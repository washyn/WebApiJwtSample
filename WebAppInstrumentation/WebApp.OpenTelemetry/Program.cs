using System.Text;

using Acme.BookStore.Web.Data;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.OpenTelemetry;

namespace Acme.BookStore.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        // Serilog.Debugging.SelfLog.Enable(Console.Error);
        // 1. Generar token Base64 para Basic Auth
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin:admin"));
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
                .WriteTo.Async(c => c.Console()).WriteTo.Async(c => c.DurableHttpUsingFileSizeRolledBuffers(
                    requestUri: "http://localhost:8888",
                    textFormatter: new Serilog.Formatting.Json.JsonFormatter()))
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
                // works
                .WriteTo.Async(a => a.Parseable(
                    "http://localhost:8000",
                    "AspNetCore",
                    "admin",
                    "admin"
                ))
                // works
                .WriteTo.Async(a => a.OpenTelemetry(options =>
                {
                    // Ruta al endpoint OTLP nativo de Parseable
                    options.Endpoint = "http://localhost:8000/v1/logs";
                    options.Protocol = OtlpProtocol.Grpc;

                    // Headers requeridos por Parseable
                    options.Headers = new Dictionary<string, string>
                    {
                        { "X-P-Stream", "fb-logs" }, { "Authorization", $"Basic {credentials}" }
                    };

                    // Atributos OTLP que viajarán con todos los logs y se volverán columnas en Parseable
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        { "service.name", "MiAplicacionDotNet" }, { "environment", "Development" }
                    };
                }))
                ///////////////////////////////////////////////////////////////////////////////////
                // .WriteTo.Async(a => a.OpenTelemetry(oltp =>
                // {
                //     // otlp.Endpoint = new Uri("http://parseable:8000/v1/logs");
                //     // otlp.Headers = "Authorization=Basic YWRtaW46YWRtaW4=,X-P-Stream=dotnet-otel";
                //     oltp.Endpoint = "http://localhost:8000/v1/logs";
                //     // oltp.Endpoint = "http://localhost:8000/api/v1/ingest";
                //     oltp.Headers = new Dictionary<string, string>()
                //     {
                //         ["Authorization"] = "Basic " +
                //                             Convert.ToBase64String(
                //                                 System.Text.Encoding.UTF8.GetBytes("admin:admin")),
                //         ["X-P-Stream"] = "AspNetCore" // should be exists in parseable
                //     };
                //     // oltp.Protocol = OtlpProtocol.HttpProtobuf;
                //     // oltp.ResourceAttributes = new Dictionary<string, object>
                //     // {
                //     //     ["service.name"] = "MyApi", ["service.version"] = "1.0.0"
                //     // };
                // }))

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
                // FOR SIGNOZ
                .WriteTo.Async(c => c.OpenTelemetry(opts =>
                {
                    opts.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["app"] = "webapi", ["runtime"] = "dotnet", ["service.name"] = "WebApi"
                    };
                    opts.Endpoint = "http://localhost:4318/v1/logs";
                    opts.Protocol = OtlpProtocol.HttpProtobuf;
                }))
                // FOR PARSEABLE
                // .WriteTo.OpenTelemetry(options =>
                // {
                //     options.Endpoint = "http://localhost:8000/v1/logs";
                //     options.Protocol = OtlpProtocol.HttpProtobuf;
                //     options.Headers = new Dictionary<string, string>
                //     {
                //         ["Authorization"] = "Basic " +
                //                             Convert.ToBase64String(
                //                                 System.Text.Encoding.UTF8.GetBytes("admin:admin")),
                //         ["X-P-Stream"] = "app-logs"
                //     };
                //     options.ResourceAttributes = new Dictionary<string, object>
                //     {
                //         ["service.name"] = "MyApi", ["service.version"] = "1.0.0"
                //     };
                // })
                // FOR PARSEABLE
                // TODO: send via http sink
// curl -X POST "http://localhost:8000/api/v1/ingest" \
//   -H "X-P-Stream: string" \
//   -H "Authorization: Basic Og==" \
//   -H "Content-Type: application/json" \
//   -d '{}'

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
            // DONE: se tiene que configurar bien el fluent-bit el input http y el output file
            // Solo funciona con el fluent-bit version 4.x no con 5.x, durable al parecer se guarda temporalmente en el disco
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
