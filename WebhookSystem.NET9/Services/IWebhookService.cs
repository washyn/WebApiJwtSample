using WebhookSystem.NET9.Models;

namespace WebhookSystem.NET9.Services
{
    public interface IWebhookService
    {
        Task<WebhookSubscription> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);
        Task<WebhookSubscription?> GetSubscriptionAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WebhookSubscription>> GetSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<WebhookSubscription?> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteSubscriptionAsync(Guid id, CancellationToken cancellationToken = default);
        Task TriggerEventAsync(WebhookEvent webhookEvent, CancellationToken cancellationToken = default);
        Task<IEnumerable<WebhookDelivery>> GetDeliveryHistoryAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
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
