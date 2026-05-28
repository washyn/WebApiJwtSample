using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Volo.Abp.Emailing;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace SmtpEmailSend
{
    [DependsOn(typeof(AbpMultiTenancyModule))]
    [DependsOn(typeof(AbpEmailingModule))]
    public class ExtraModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<ICustomEmailSender, MailKitEmailSender>();
            context.Services.AddHostedService<MainService>();
        }
    }

    public class MainService : IHostedService
    {
        private readonly ILogger<MainService> _logger;
        private readonly ICustomEmailSender _emailSender;

        public MainService(ILogger<MainService> logger, ICustomEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainService started. Starting email test with CUSTOM sender...");

            try
            {
                // Este es un correo de prueba usando el remitente CUSTOM (MailKit).
                await _emailSender.SendAsync(
                    to: "oti.asistente2@unaj.edu.pe",
                    subject: "Prueba de envío de correo PoC (CUSTOM)",
                    body:
                    "Hola! Este es un correo enviado desde la PoC usando un IEmailSender CUSTOM (MailKit) sin depender de la implementación de ABP."
                );

                _logger.LogInformation("Custom Email sent successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending custom email. Check your settings in appsettings.json.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainService stopped.");
            return Task.CompletedTask;
        }
    }
}