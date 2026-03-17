using System;
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
        }
    }

    public class MyProjectNameHostedService : IHostedService
    {
        private readonly IAbpApplicationWithExternalServiceProvider _application;

        private readonly ILogger<MyProjectNameHostedService> _logger;
        private readonly IOptions<App> _appOptions;

        // private readonly SampleService _sampleService;
        private readonly IServiceProvider _serviceProvider;

        public MyProjectNameHostedService(
            IAbpApplicationWithExternalServiceProvider application,
            ILogger<MyProjectNameHostedService> logger,
            IOptions<App> appOptions,
            // SampleService sampleService,
            IServiceProvider serviceProvider)
        {
            _application = application;
            _logger = logger;
            _appOptions = appOptions;
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