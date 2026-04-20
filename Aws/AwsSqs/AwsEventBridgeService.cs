using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AwsSqs;

public class AwsEventBridgeService
{
    private readonly AwsEventBridgeConfig _options;
    private readonly ILogger<AwsEventBridgeService> _logger;

    public AwsEventBridgeService(IOptions<AwsEventBridgeConfig> options,
        ILogger<AwsEventBridgeService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task PublishEventAsync()
    {
        var isLocalStack = true;
        _logger.LogInformation($"Publishing event to EventBridge...");
        // display optins
        _logger.LogInformation("Options:{@options} ", _options);
        var config = new AmazonEventBridgeConfig { ServiceURL = "http://localhost:4566" };
        var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);
        var region = Amazon.RegionEndpoint.GetBySystemName(_options.Region);

        AmazonEventBridgeClient eventBridgeClient;
        if (isLocalStack)
        {
            eventBridgeClient = new AmazonEventBridgeClient(credentials, region);
        }
        else
        {
            eventBridgeClient = new AmazonEventBridgeClient(credentials, config);
        }

        var putEventsRequest = new PutEventsRequest
        {
            Entries = new List<PutEventsRequestEntry>
            {
                new PutEventsRequestEntry
                {
                    EventBusName = _options.EventBusName, // arn of event bus
                    Source = _options.Source,
                    DetailType = "product.created",
                    Detail = "hiiiiiiiiiiiiiiii"
                }
            }
        };

        try
        {
            var response = await eventBridgeClient.PutEventsAsync(putEventsRequest);
            if (response.FailedEntryCount > 0)
            {
                _logger.LogWarning($"Error al enviar {response.FailedEntryCount} evento(s).");
            }

            _logger.LogInformation($"Eventos enviados: {response.Entries.Count}");

            foreach (var entry in response.Entries)
            {
                if (!string.IsNullOrEmpty(entry.EventId))
                {
                    _logger.LogInformation($"Evento OK: {entry.EventId}");
                }
                else
                {
                    _logger.LogError($"Error: {entry.ErrorCode} - {entry.ErrorMessage}");
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}

public class AwsEventBridgeConfig
{
    // add required config properties for bridge
    public string Region { get; set; } = "us-east-1";

    public string EventBusName { get; set; } = "mi-topic"; // arn of event bus
    public string Source { get; set; } = "delosi.products"; // source of event bus

    // accessKey
    // SecretKet
    public string AccessKeyId { get; set; } = "test";
    public string SecretAccessKey { get; set; } = "test";
}