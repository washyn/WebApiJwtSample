using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using MimeKit;

namespace SmtpEmailSend
{
    // thows email second  factor auth
    public class MailKitEmailSender : ICustomEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(IConfiguration configuration, ILogger<MailKitEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var host = _configuration["Settings:Abp.Mailing.Smtp.Host"];
            var portStr = _configuration["Settings:Abp.Mailing.Smtp.Port"];
            var userName = _configuration["Settings:Abp.Mailing.Smtp.UserName"];
            var password = _configuration["Settings:Abp.Mailing.Smtp.Password"];
            var enableSsl = _configuration["Settings:Abp.Mailing.Smtp.EnableSsl"]?.ToLower() == "true";
            var fromAddress = _configuration["Settings:Abp.Mailing.DefaultFromAddress"];
            var fromName = _configuration["Settings:Abp.Mailing.DefaultFromDisplayName"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portStr))
            {
                throw new Exception("SMTP settings are not properly configured in appsettings.json.");
            }

            var port = int.Parse(portStr);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { TextBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                _logger.LogInformation($"Connecting to {host}:{port}...");
                await client.ConnectAsync(host, port,
                    enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                _logger.LogInformation($"Authenticating as {userName}...");
                await client.AuthenticateAsync(userName, password);

                _logger.LogInformation("Sending email...");
                await client.SendAsync(message);

                await client.DisconnectAsync(true);
                _logger.LogInformation("Email sent successfully using custom MailKit sender.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via MailKit.");
                throw;
            }
        }
    }
}
