using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Volo.Abp.Modularity;

namespace SmtpEmailSend
{
    public class ExtraModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHostedService<MainService>();
        }
    }

    public class MainService : IHostedService
    {
        private readonly ILogger<MainService> _logger;

        public MainService(ILogger<MainService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}