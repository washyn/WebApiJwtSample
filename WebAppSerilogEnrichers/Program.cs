using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Washyn.BookStore.Data;
using Serilog;
using Serilog.Enrichers.CallerInfo;
using Serilog.Enrichers.CallStack;
using Serilog.Enrichers.HttpContextData;
using Serilog.Enrichers.OpenTracing;
using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Enrichers.SqlException.Extensions;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;
using Volo.Abp.Data;
using Volo.Abp.Security.Claims;

namespace Washyn.BookStore
{
    public class Program
    {
        public async static Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()// add
                .Enrich.WithEnvironmentName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessName()
                .Enrich.WithSpan()
                .Enrich.WithCorrelationId()
                .Enrich.WithClientAgent()
                .Enrich.WithClientIp()
                .Enrich.WithSensitiveDataMasking(opt => { opt.Mode = MaskingMode.InArea; })
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithAssemblyInformationalVersion()
                .Enrich.WithMemoryUsage()// add
                .Enrich.WithDemystifiedStackTraces() // add
                .Enrich.FromGlobalLogContext()
                // .Enrich.WithDynamic()
                .Enrich.WithExceptionData() // add
                .Enrich.WithExceptionStackTraceHash()
                .Enrich.WithOpenTracingContext()
                .Enrich.WithHttpContextData() // add
                .Enrich.WithCallerInfo(true, new List<string>(), "pref")
                .Enrich.WithTraceIdentifier()
                .Enrich.WithCallStack() // add
                .Enrich.WithRequestUserId()
                .Enrich.WithHttpHeader("content/type", "application/json")
                .Enrich.WithSqlExceptionEnricher() // add
                .Enrich.WithCustomClaim(AbpClaimTypes.TenantId)
                .WriteTo.Async(c => c.File(
                    formatter: new Serilog.Formatting.Json.JsonFormatter(),
                    path: "Logs/logs.log", rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 2))
                .WriteTo.Async(c => c.Console())
                .WriteTo.Async(c => c.OpenTelemetry(options =>
                {
                    options.Endpoint = "http://localhost:4318";
                    options.Protocol = OtlpProtocol.HttpProtobuf;
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = "washyn-bookstore",
                        ["service.namespace"] = "Washyn.BookStore",
                        ["service.instance.id"] = Environment.MachineName
                    };
                }))
                .CreateLogger();

            try
            {
                Log.Information("Starting Washyn.BookStore.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Washyn.BookStore terminated unexpectedly!");
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

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(build => { build.AddJsonFile("appsettings.secrets.json", optional: true); })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseAutofac()
                .UseSerilog();
    }
}
