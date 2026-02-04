using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using WebhookSystem.NET9.Data;
using WebhookSystem.NET9.Models;

namespace WebhookSystem.NET9.Services
{
    /// <summary>
    /// Interface for sending webhooks, http client.
    /// </summary>
    public interface IWebhookSender
    {
        Task SendWebhookAsync(WebhookSubscription subscription, WebhookEvent webhookEvent,
            CancellationToken cancellationToken = default);

        Task RetryFailedWebhookAsync(Guid deliveryId, CancellationToken cancellationToken = default);
    }

    // IMRPOVEMENT: can be improve key share process, el secret no se deberi compartir via header
    // IMRPOVEMENT: add another proyect for test
    // TODO: implement send with retry standalone with hangfire and then use ai sugestion
    public class WebhookSender : IWebhookSender
    {
        private readonly HttpClient _httpClient;
        private readonly WebhookDbContext _context;
        private readonly IHmacAuthenticationService _hmacService;
        private readonly ILogger<WebhookSender> _logger;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public WebhookSender(
            HttpClient httpClient,
            WebhookDbContext context,
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
                AttemptNumber = 1
            };
            try
            {
                await SendWithRetryAsync(subscription, delivery, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send webhook to {Url} for subscription {SubscriptionId}",
                    subscription.Url, subscription.Id);
            }
        }

        public async Task RetryFailedWebhookAsync(Guid deliveryId, CancellationToken cancellationToken = default)
        {
            var delivery = await _context.Deliveries
                .Include(d => d.Subscription)
                .FirstOrDefaultAsync(d => d.Id == deliveryId, cancellationToken);
            if (delivery == null || delivery.IsSuccessful)
            {
                _logger.LogWarning("Delivery {DeliveryId} not found or already successful", deliveryId);
                return;
            }

            if (delivery.AttemptNumber >= delivery.Subscription.MaxRetries)
            {
                _logger.LogWarning("Max retries exceeded for delivery {DeliveryId}", deliveryId);
                return;
            }

            delivery.AttemptNumber++;
            delivery.AttemptedAt = DateTime.UtcNow;
            try
            {
                await SendWebhookRequestAsync(delivery.Subscription, delivery, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retry webhook delivery {DeliveryId}", deliveryId);
            }
        }

        // TODO: implement, and when not success trow error for retry by hangfire
        // Can be add fix for send only one webhook without retry
        // TODO: use with retry hangfire decorator
        private async Task SendAsync(
            WebhookSubscription subscription,
            WebhookDelivery delivery,
            CancellationToken cancellationToken)
        {
            var maxAttempts = subscription.MaxRetries;
            var retryDelay = subscription.RetryDelay;
            delivery.AttemptNumber = 1;
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
                _logger.LogWarning(ex, "Webhook delivery attempt {Attempt}/{MaxAttempts} failed for {Url}",
                    1, maxAttempts, subscription.Url);
            }

            _logger.LogInformation("Delivery {@Delivery}", delivery);
            // Save attempt result
            if (delivery.Id == Guid.Empty)
            {
                _context.Deliveries.Add(delivery);
            }
            else
            {
                _context.Deliveries.Update(delivery);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SendWithRetryAsync(
            WebhookSubscription subscription,
            WebhookDelivery delivery,
            CancellationToken cancellationToken)
        {
            var maxAttempts = subscription.MaxRetries;
            var retryDelay = subscription.RetryDelay;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                delivery.AttemptNumber = attempt;
                delivery.AttemptedAt = DateTime.UtcNow;
                try
                {
                    await SendWebhookRequestAsync(subscription, delivery, cancellationToken);
                    if (delivery.IsSuccessful)
                    {
                        _logger.LogInformation("Webhook delivered successfully to {Url} on attempt {Attempt}",
                            subscription.Url, attempt);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    delivery.ErrorMessage = ex.Message;
                    _logger.LogWarning(ex, "Webhook delivery attempt {Attempt}/{MaxAttempts} failed for {Url}",
                        attempt, maxAttempts, subscription.Url);
                }

                _logger.LogInformation("Delivery {@Delivery}", delivery);
                // Save attempt result
                if (delivery.Id == Guid.Empty)
                {
                    _context.Deliveries.Add(delivery);
                }
                else
                {
                    _context.Deliveries.Update(delivery);
                }

                await _context.SaveChangesAsync(cancellationToken);
                // Wait before retry (except for last attempt)
                if (attempt < maxAttempts && !delivery.IsSuccessful)
                {
                    var delayMs = (int)(retryDelay.TotalMilliseconds * Math.Pow(2, attempt - 1)); // Exponential backoff
                    await Task.Delay(Math.Min(delayMs, 300000), cancellationToken); // Max 5 minutes
                }
            }
        }

        private async Task SendWebhookRequestAsync(
            WebhookSubscription subscription,
            WebhookDelivery delivery,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, subscription.Url);

            // Set content
            request.Content = new StringContent(delivery.Payload, Encoding.UTF8, "application/json");
            // Add custom headers
            foreach (var header in subscription.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Add authentication headers
            var timestamp = _hmacService.GenerateTimestamp();
            var signature = _hmacService.GenerateSignature(delivery.Payload, subscription.Secret);
            // IMRPOVEMENT: this secret aded only for test
            request.Headers.Add("X-API-Key", subscription.Secret);
            request.Headers.Add("X-Webhook-Signature", $"sha256={signature}");
            request.Headers.Add("X-Webhook-Timestamp", timestamp);
            request.Headers.Add("X-Webhook-Id", delivery.Id.ToString());
            request.Headers.Add("User-Agent", "WebhookSystem.NET9/1.0");
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using var response = await _httpClient.SendAsync(request, cancellationToken);
                stopwatch.Stop();
                delivery.HttpStatusCode = (int)response.StatusCode;
                delivery.ResponseTime = stopwatch.Elapsed;
                delivery.IsSuccessful = response.IsSuccessStatusCode;
                // Read response content
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    delivery.Response = responseContent.Length > 10000
                        ? responseContent[..10000] + "..." // Truncate large responses
                        : responseContent;
                }

                if (!response.IsSuccessStatusCode)
                {
                    delivery.ErrorMessage = $"HTTP {delivery.HttpStatusCode}: {response.ReasonPhrase}";
                }
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                delivery.ResponseTime = stopwatch.Elapsed;
                delivery.ErrorMessage = ex.Message;
                delivery.IsSuccessful = false;
                throw;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                stopwatch.Stop();
                delivery.ResponseTime = stopwatch.Elapsed;
                delivery.ErrorMessage = "Request timeout";
                delivery.IsSuccessful = false;
                throw;
            }
        }

        private static WebhookPayload CreatePayload(WebhookEvent webhookEvent)
        {
            return new WebhookPayload(
                webhookEvent.Id.ToString(),
                webhookEvent.EventType,
                webhookEvent.Source,
                webhookEvent.Timestamp,
                webhookEvent.Version,
                webhookEvent.Data,
                webhookEvent.Metadata
            );
        }
    }
}
