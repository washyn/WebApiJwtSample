using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Serilog;

using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Server
{
    [DependsOn(typeof(AbpAutofacModule))]
    public class MyProjectNameModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            foreach (var descriptor in context.Services)
            {
                Log.Information(
                    $"Service: {descriptor.ServiceType.Name}, Implementation: {descriptor.ImplementationType?.Name ?? "N/A"}, Lifetime: {descriptor.Lifetime}");
            }

            Log.Information($"Services count: {context.Services.Count}");
            var configuration = context.Services.GetConfiguration();
            var hostEnvironment = context.Services.GetSingletonInstance<IHostEnvironment>();
            context.Services.AddHostedService<MyProjectNameHostedService>();
            context.Services.AddOptions<App>()
                .BindConfiguration(nameof(App));
            context.Services.AddHttpClient();
        }
    }

    public class MyProjectNameHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;

        private readonly ILogger<MyProjectNameHostedService> _logger;
        private readonly IOptions<App> _appOptions;
        private readonly IHttpClientFactory _httpClientFactory;

        // private readonly SampleService _sampleService;
        private readonly IServiceProvider _serviceProvider;

        public MyProjectNameHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            ILogger<MyProjectNameHostedService> logger,
            IOptions<App> appOptions,
            IHttpClientFactory httpClientFactory,
            // SampleService sampleService,
            IServiceProvider serviceProvider)
        {
            _application = application;
            _logger = logger;
            _appOptions = appOptions;
            _httpClientFactory = httpClientFactory;
            // _sampleService = sampleService;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Initialize(_serviceProvider);
            _logger.LogInformation("MyProjectName module is initialized.");
            _logger.LogInformation($"App Value: {_appOptions.Value.Value}");
            // await _sampleService.ShowExampleData();
            // TODO: compress some directory and send to agent
            var directory = @"F:\maestria";
            var zipFilePath = @"F:\maestria.zip";
            using (var zipToOpen = new FileStream(zipFilePath, FileMode.Create))
            using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                archive.CreateEntry(directory, CompressionLevel.SmallestSize);
            }

            _logger.LogInformation($"Directory {directory} compressed to {zipFilePath}");

            // TODO: send zip file to agent
            using var form = new MultipartFormDataContent();

            // archivo
            var fileStream = File.OpenRead(zipFilePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            form.Add(fileContent, "File", Path.GetFileName(zipFilePath));

            // campos normales
            form.Add(new StringContent("MiAplicacion"), "NameApp");
            form.Add(new StringContent("DefaultAppPool"), "PoolName");

            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.PostAsync(
                "http://localhost:5151/app/deploy",
                form
            );
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response: {responseContent}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();
            return Task.CompletedTask;
        }
    }

    public class App
    {
        public string Value { get; set; }
    }
}