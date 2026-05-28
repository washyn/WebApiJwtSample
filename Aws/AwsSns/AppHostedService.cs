using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AwsSns;

public class AppHostedService : IHostedService
{
    private readonly ILogger<AppHostedService> _logger;
    private readonly AwsSnsConfig _options;

    public AppHostedService(ILogger<AppHostedService> logger, IOptions<AwsSnsConfig> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var isLocalStack = true;
        AmazonSimpleNotificationServiceClient client;
        if (isLocalStack)
        {
            var config = new AmazonSimpleNotificationServiceConfig
            {
                ServiceURL = "http://localhost:4566"
            };

            var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);

            client = new AmazonSimpleNotificationServiceClient(credentials, config);
        }
        else
        {
            client = new AmazonSimpleNotificationServiceClient(_options.AccessKeyId, _options.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(_options.Region));
        }

        var request = new PublishRequest
        {
            TopicArn = _options.TopicArn,
            Message = "Hola desde C# usando SNS",
            Subject = "Notificación de prueba"
        };

        try
        {
            var response = await client.PublishAsync(request);
            _logger.LogInformation($"Mensaje enviado. MessageId: {response.MessageId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar mensaje a SNS");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class AwsSnsConfig
{
    public string TopicArn { get; set; }
    public string Region { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
}