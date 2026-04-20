using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

namespace AwsSqs
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
                    services.AddHostedService<AwsSqsService>();
                    services.Configure<AwsEventBridgeConfig>(a => { });
                    services.Configure<AwsSqsConfig>(a =>
                    {
                        a.Region = "us-east-1";
                        a.AccessKeyId = "test";
                        a.SecretAccessKey = "test";
                        a.QueueUrl = "http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/mi-cola-1";
                    });
                });
    }

    public class AwsSqsConfig
    {
        public string QueueUrl { get; set; }
        public string Region { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
    }

    public class AwsSqsService : IHostedService
    {
        private readonly ILogger<AwsSqsService> _logger;
        private readonly AwsSqsConfig _options;

        public AwsSqsService(ILogger<AwsSqsService> logger, IOptions<AwsSqsConfig> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
