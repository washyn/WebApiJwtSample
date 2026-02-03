using WebhookSystem.NET9.Models;

namespace WebhookSystem.NET9.Services
{
    public interface IWebhookSender
    {
        Task SendWebhookAsync(WebhookSubscription subscription, WebhookEvent webhookEvent, CancellationToken cancellationToken = default);
        Task RetryFailedWebhookAsync(Guid deliveryId, CancellationToken cancellationToken = default);
    }
}
