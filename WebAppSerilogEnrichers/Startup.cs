using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.IO;
using Volo.Abp.Modularity.PlugIns;
using Washyn.BookStore;

namespace Washyn.BookStore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? "http://localhost:4318";

            services.AddApplication<BookStoreModule>(options =>
            {
                // var path = options.Services.GetHostingEnvironment().ContentRootPath;
                // var directoryInfo = new DirectoryInfo(path);
                // var folder = string.Empty;
                // folder = Path.Combine(directoryInfo.FullName, "plugins");
                // options.PlugInSources.AddFolder(folder);
            });

            services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource.AddService("washyn-bookstore", serviceNamespace: "Washyn.BookStore");
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri($"{otlpEndpoint}/v1/traces");
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri($"{otlpEndpoint}/v1/metrics");
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.InitializeApplication();
        }
    }
}
