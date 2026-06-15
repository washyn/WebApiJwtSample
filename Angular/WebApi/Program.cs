using Acme.BookStore.Web.Data;

using Serilog;
using Serilog.Events;

namespace WebApp;

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
            .WriteTo.Async(c => c.File("Logs/log_.log", rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7, shared: true, restrictedToMinimumLevel: LogEventLevel.Information))
            .WriteTo.Async(c => c.Console());

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
                .UseSerilog();
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

            Log.Information("Starting Acme.BookStore.Web.");
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            // if (ex is HostAbortedException)
            // {
            //     throw;
            // }

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
