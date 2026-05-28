using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace AwsSqsReceiver
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.log"
                    , rollingInterval: RollingInterval.Day
                    , retainedFileCountLimit: 2))
                .WriteTo.Async(c => c.Console())
                .CreateLogger();

            try
            {
                Log.Information("Starting console host.");
                await CreateHostBuilder(args).RunConsoleAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment("Development")
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<AppHostedService>();
                    services.Configure<AwsSqsConfig>(a =>
                    {
                        a.QueueUrl = "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/mi-cola-1";
                        a.AccessKeyId = "test";
                        a.SecretAccessKey = "test";
                        a.Region = "us-east-1";
                    });
                });
    }
}

public class AppHostedService : BackgroundService
{
    private readonly ILogger<AppHostedService> _logger;
    private readonly AwsSqsConfig _options;

    public AppHostedService(IOptions<AwsSqsConfig> options, ILogger<AppHostedService> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Starting polling on queue: {_options.QueueUrl}");
        var isLocalStack = true;
        AmazonSQSClient client;
        if (isLocalStack)
        {
            var credentials = new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey);
            var config = new AmazonSQSConfig()
            {
                ServiceURL = "http://localhost:4566", // endpoint de LocalStack
                UseHttp = true
            };
            client = new AmazonSQSClient(credentials, config);
        }
        else
        {
            client = new AmazonSQSClient(_options.AccessKeyId, _options.SecretAccessKey,
                Amazon.RegionEndpoint.GetBySystemName(_options.Region));
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest()
            {
                QueueUrl = _options.QueueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 20
            });

            foreach (var message in response.Messages)
            {
                _logger.LogInformation($"Received message: {message.Body} (MessageId: {message.MessageId})");
                _logger.LogInformation("Message : {@message}", message);
                await client.DeleteMessageAsync(new DeleteMessageRequest()
                {
                    QueueUrl = _options.QueueUrl,
                    ReceiptHandle = message.ReceiptHandle
                });
                _logger.LogInformation($"Deleted message: {message.MessageId}");
            }
        }
    }
}

public class AwsSqsConfig
{
    public string QueueUrl { get; set; }
    public string Region { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
}