using Acme.BookStore.Web.Data;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.OpenTelemetry;

namespace Acme.BookStore.Web;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        // TODO: levantar fb input de otel -> victoria otel input
        // o fb tener 3 inputs
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
                .WriteTo.Async(c => c.File(new JsonFormatter(), path: "Logs/logs.log")) // success
                // .WriteTo.Async(a => a.OpenTelemetry(options =>
                // {
                //     // Ruta al endpoint OTLP nativo de Parseable
                //     options.Endpoint = "http://localhost:8000/v1/logs";
                //     options.Protocol = OtlpProtocol.Grpc;
                // }))
                .WriteTo.Async(a => a.OpenTelemetry())
                // .WriteTo.Async(a => a.OpenObserve(
                //     "http://localhost:5080/api/default",
                //     "default",
                //     "root@example.com",
                //     "Complexpass#123"
                // ))
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
