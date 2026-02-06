using System.Text;
using System.Text.Json;

using Lib;

using Sender.Data;
using Sender.Models;

namespace Sender
{
    public interface IWebhookSender
    {
        Task SendWebhookAsync(WebhookSubscription subscription, WebhookEvent webhookEvent,
            CancellationToken cancellationToken = default);
    }

    public class WebhookSender : IWebhookSender
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly IHmacAuthenticationService _hmacService;
        private readonly ILogger<WebhookSender> _logger;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false
        };

        public WebhookSender(
            HttpClient httpClient,
            AppDbContext context,
            IHmacAuthenticationService hmacService,
            ILogger<WebhookSender> logger)
        {
            _httpClient = httpClient;
            _context = context;
            _hmacService = hmacService;
            _logger = logger;
        }

        public async Task SendWebhookAsync(
            WebhookSubscription subscription,
            WebhookEvent webhookEvent,
            CancellationToken cancellationToken = default)
        {
            var payload = CreatePayload(webhookEvent);
            var payloadJson = JsonSerializer.Serialize(payload, SerializerOptions);
            var delivery = new WebhookDelivery
            {
                SubscriptionId = subscription.Id,
                EventId = webhookEvent.Id,
                Url = subscription.Url,
                Payload = payloadJson,
            };
            try
            {
                await SendAsync(subscription, delivery, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send webhook to {Url} for subscription {SubscriptionId}",
                    subscription.Url, subscription.Id);
            }
        }

        // TODO: implement, and when not success trow error for retry by hangfire, can be use with retry hangfire decorator
        private async Task SendAsync(
            WebhookSubscription subscription,
            WebhookDelivery delivery,
            CancellationToken cancellationToken)
        {
            delivery.AttemptedAt = DateTime.UtcNow;
            try
            {
                await SendWebhookRequestAsync(subscription, delivery, cancellationToken);
                if (delivery.IsSuccessful)
                {
                    _logger.LogInformation("Webhook delivered successfully to {Url} on attempt {Attempt}",
                        subscription.Url, 1);
                }
            }
            catch (Exception ex)
            {
                delivery.ErrorMessage = ex.Message;
                _logger.LogWarning(ex, "Webhook delivery failed for {Url}", subscription.Url);
            }

            _logger.LogInformation("Delivery {@Delivery}", delivery);
            // Save attempt result
            if (delivery.Id == Guid.Empty)
            {
                _context.Deliveries.Add(delivery);
            }
            // TODO: validate an sure if this works, and how to works
            // if this is retry,
            else
            {
                _context.Deliveries.Update(delivery);
            }

            // await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SendWebhookRequestAsync(
            WebhookSubscription subscription,
            WebhookDelivery delivery,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Url);

            // Set content
            request.Content = new StringContent(delivery.Payload, Encoding.UTF8,
                System.Net.Mime.MediaTypeNames.Application.Json);
            // Add custom headers
            foreach (var header in subscription.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Add authentication headers
            var timestamp = _hmacService.GenerateTimestamp();
            var signature = _hmacService.GenerateSignature(delivery.Payload, subscription.Secret);
            request.Headers.Add("X-Webhook-Signature", $"sha256={signature}");
            request.Headers.Add("X-Webhook-Timestamp", timestamp);
            request.Headers.Add("X-Webhook-Id", delivery.Id.ToString());
            try
            {
                using var response = await _httpClient.SendAsync(request, cancellationToken);
                delivery.HttpStatusCode = response.StatusCode.ToInt();
                delivery.IsSuccessful = response.IsSuccessStatusCode;
                // TODO: check content of receiver response
                // var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                // delivery.Response = responseContent;

                if (!response.IsSuccessStatusCode)
                {
                    delivery.ErrorMessage = $"HTTP {delivery.HttpStatusCode}: {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                delivery.ErrorMessage = ex.Message;
                delivery.IsSuccessful = false;
                throw;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                delivery.ErrorMessage = "Request timeout";
                delivery.IsSuccessful = false;
                throw;
            }
        }

        private static WebhookPayload CreatePayload(WebhookEvent webhookEvent)
        {
            return new WebhookPayload()
            {
                Id = webhookEvent.Id.ToString(),
                Event = webhookEvent.EventType,
                Source = webhookEvent.Source,
                Timestamp = webhookEvent.Timestamp,
                Version = webhookEvent.Version,
                Data = webhookEvent.Data,
                Metadata = webhookEvent.Metadata
            };
        }
    }

    public static class EnumExtensions
    {
        public static int ToInt<TEnum>(this TEnum value)
            where TEnum : struct, Enum
        {
            return Convert.ToInt32(value);
        }
    }
}
