using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json.Newtonsoft;
using Volo.Abp.Modularity;

namespace AwsSqs;

[DependsOn(typeof(AbpAutofacModule))]
[DependsOn(typeof(AbpJsonNewtonsoftModule))]
public class AppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHostedService<AppHostedService>();
        Configure<AwsEventBridgeConfig>(a => { });
        Configure<AwsSqsConfig>(a =>
        {
            a.Region = "us-east-1";
            a.AccessKeyId = "test";
            a.SecretAccessKey = "test";
            a.QueueUrl = "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/mi-cola-1";
        });
    }
}

public class AppHostedService : IHostedService
{
    private readonly IAbpApplicationWithExternalServiceProvider _application;
    private readonly ILogger<AppHostedService> _logger;
    private readonly AwsEventBridgeService _eventBridgeService;
    private readonly AwsSqsService _sqsService;
    private readonly IServiceProvider _serviceProvider;

    public AppHostedService(
        IAbpApplicationWithExternalServiceProvider application,
        ILogger<AppHostedService> logger,
        AwsEventBridgeService eventBridgeService,
        AwsSqsService sqsService,
        IServiceProvider serviceProvider)
    {
        _application = application;
        _logger = logger;
        _eventBridgeService = eventBridgeService;
        _sqsService = sqsService;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _application.Initialize(_serviceProvider);
        _logger.LogInformation("MyProjectName module is initialized.");
        // code here ...
        // await _eventBridgeService.PublishEventAsync();
        await _sqsService.PublishMessageAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _application.Shutdown();
        return Task.CompletedTask;
    }
}

public class AwsSqsConfig
{
    public string QueueUrl { get; set; }
    public string Region { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
}

public class AwsSqsService : ITransientDependency
{
    private readonly ILogger<AwsSqsService> _logger;
    private readonly AwsSqsConfig _options;

    public AwsSqsService(ILogger<AwsSqsService> logger, IOptions<AwsSqsConfig> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task PublishMessageAsync()
    {
        var isLocalStack = true;
        // add example 
        _logger.LogInformation($"Publishing message to SQS...");

        AmazonSQSClient client;

        if (isLocalStack)
        {
            var config = new AmazonSQSConfig
            {
                ServiceURL = "http://localhost:4566", // endpoint de LocalStack
                UseHttp = true
            };

            // Credenciales dummy (LocalStack acepta cualquiera)
            var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);

            client = new AmazonSQSClient(credentials, config);
        }
        else
        {
            client = new AmazonSQSClient(_options.AccessKeyId, _options.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(_options.Region));
        }

        var request = new SendMessageRequest
        {
            QueueUrl = _options.QueueUrl,
            MessageBody = "Hola desde C# hacia SQS",
        };

        try
        {
            var response = await client.SendMessageAsync(request);

            _logger.LogInformation($"Mensaje enviado. MessageId: {response.MessageId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
    }
}