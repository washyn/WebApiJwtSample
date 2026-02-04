using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WebhookSystem.NET9.Data;
using WebhookSystem.NET9.Models;

namespace WebhookSystem.NET9.Services
{
    // For manage subscriptions
    public interface IWebhookService
    {
        Task<WebhookSubscription> CreateSubscriptionAsync(CreateSubscriptionRequest request,
            CancellationToken cancellationToken = default);

        Task<WebhookSubscription?> GetSubscriptionAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WebhookSubscription>> GetSubscriptionsAsync(CancellationToken cancellationToken = default);

        Task<WebhookSubscription?> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteSubscriptionAsync(Guid id, CancellationToken cancellationToken = default);
        Task TriggerEventAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<WebhookDelivery>> GetDeliveryHistoryAsync(Guid subscriptionId,
            CancellationToken cancellationToken = default);
    }

    public class WebhookService : IWebhookService
    {
        private readonly WebhookDbContext _context;
        private readonly IWebhookSender _webhookSender;
        private readonly ILogger<WebhookService> _logger;

        public WebhookService(
            WebhookDbContext context,
            IWebhookSender webhookSender,
            ILogger<WebhookService> logger)
        {
            _context = context;
            _webhookSender = webhookSender;
            _logger = logger;
        }

        public async Task<WebhookSubscription> CreateSubscriptionAsync(
            CreateSubscriptionRequest request,
            CancellationToken cancellationToken = default)
        {
            var subscription = new WebhookSubscription
            {
                Url = request.Url,
                Events = request.Events,
                Description = request.Description,
                Headers = request.Headers ?? new Dictionary<string, string>(),
                MaxRetries = request.MaxRetries,
                RetryDelay = TimeSpan.FromMinutes(request.RetryDelayMinutes),
                Secret = GenerateSecret()
            };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created webhook subscription {SubscriptionId} for URL {Url}",
                subscription.Id, subscription.Url);
            return subscription;
        }

        public async Task<WebhookSubscription?> GetSubscriptionAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Subscriptions
                .Include(s => s.Deliveries.OrderByDescending(d => d.AttemptedAt).Take(100))
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Subscriptions
                .Where(s => s.IsActive)
                .OrderBy(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<WebhookSubscription?> UpdateSubscriptionAsync(
            Guid id,
            UpdateSubscriptionRequest request,
            CancellationToken cancellationToken = default)
        {
            var subscription = await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
            if (subscription == null)
                return null;
            if (!string.IsNullOrEmpty(request.Url))
                subscription.Url = request.Url;
            if (request.Events != null)
                subscription.Events = request.Events;
            if (request.IsActive.HasValue)
                subscription.IsActive = request.IsActive.Value;
            if (request.Description != null)
                subscription.Description = request.Description;
            if (request.Headers != null)
                subscription.Headers = request.Headers;
            if (request.MaxRetries.HasValue)
                subscription.MaxRetries = request.MaxRetries.Value;
            if (request.RetryDelayMinutes.HasValue)
                subscription.RetryDelay = TimeSpan.FromMinutes(request.RetryDelayMinutes.Value);
            subscription.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated webhook subscription {SubscriptionId}", subscription.Id);
            return subscription;
        }

        public async Task<bool> DeleteSubscriptionAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var subscription = await _context.Subscriptions.FindAsync(new object[] { id }, cancellationToken);
            if (subscription == null)
                return false;
            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted webhook subscription {SubscriptionId}", id);
            return true;
        }

        // desencadenador de evento para pruebas
        public async Task TriggerEventAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default)
        {
            //var subscriptions = await _context.Subscriptions
            //    .Where(s => s.IsActive && s.Events.Any(p => p == webhookEvent.EventType))
            //    .ToListAsync(cancellationToken);

            // Paso 1: Traer todas las suscripciones activas desde la BD
            var activeSubscriptions = await _context.Subscriptions
                .Where(s => s.IsActive)
                .ToListAsync(cancellationToken);

            // Paso 2: Filtrar en memoria por el evento específico
            var subscriptions = activeSubscriptions
                .Where(s => s.Events.Contains(webhookEvent.EventType))
                .ToList();

            if (!subscriptions.Any())
            {
                _logger.LogDebug("No active subscriptions found for event type {EventType}", webhookEvent.EventType);
                return;
            }

            _logger.LogInformation("Triggering webhook event {EventType} to {Count} subscriptions",
                webhookEvent.EventType, subscriptions.Count);
            var tasks = subscriptions.Select(subscription =>
                _webhookSender.SendWebhookAsync(subscription, webhookEvent, cancellationToken));
            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<WebhookDelivery>> GetDeliveryHistoryAsync(
            Guid subscriptionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Deliveries
                .Where(d => d.SubscriptionId == subscriptionId)
                .OrderByDescending(d => d.AttemptedAt)
                .Take(1000) // Limit to last 1000 deliveries
                .ToListAsync(cancellationToken);
        }

        private static string GenerateSecret()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public record CreateSubscriptionRequest(
        string Url,
        List<string> Events,
        string? Description = null,
        Dictionary<string, string>? Headers = null,
        int MaxRetries = 3,
        int RetryDelayMinutes = 1
    );

    public record UpdateSubscriptionRequest(
        string? Url = null,
        List<string>? Events = null,
        bool? IsActive = null,
        string? Description = null,
        Dictionary<string, string>? Headers = null,
        int? MaxRetries = null,
        int? RetryDelayMinutes = null
    );
}
