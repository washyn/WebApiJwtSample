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
        private readonly App _appOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceProvider _serviceProvider;

        public MyProjectNameHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            ILogger<MyProjectNameHostedService> logger,
            IOptions<App> appOptions,
            IHttpClientFactory httpClientFactory,
            IServiceProvider serviceProvider)
        {
            _application = application;
            _logger = logger;
            _appOptions = appOptions.Value;
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
        }

        // TODO: improve for test
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (File.Exists(@"E:\tesis_maestria.zip"))
            {
                File.Delete(@"E:\tesis_maestria.zip");
            }

            _application.Initialize(_serviceProvider);
            _logger.LogInformation("MyProjectName module is initialized.");

            // Comprimir directorio
            ZipFile.CreateFromDirectory(@"E:\tesis_maestria", @"E:\tesis_maestria.zip");


            // Enviar zip al agente
            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(@"E:\tesis_maestria.zip");
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            form.Add(fileContent, "File", Path.GetFileName(@"E:\tesis_maestria.zip"));
            form.Add(new StringContent("MiAplicacion"), "NameApp");
            form.Add(new StringContent("DefaultAppPool"), "PoolName");

            var httpClient = _httpClientFactory.CreateClient();
            _logger.LogInformation($"Enviando paquete a {_appOptions.AgentUrl}...");
            var response = await httpClient.PostAsync(_appOptions.AgentUrl, form, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogInformation($"Despliegue exitoso: {responseContent}");
            }
            else
            {
                _logger.LogError($"Error en el despliegue. Status: {response.StatusCode}");
            }

            if (File.Exists(@"E:\tesis_maestria.zip"))
            {
                // File.Delete(@"E:\tesis_maestria.zip");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Shutdown();
            return Task.CompletedTask;
        }
    }

    public class App
    {
        public string AgentUrl { get; set; }
        public string SourceDirectory { get; set; }
        public string ZipFileName { get; set; }
    }
}