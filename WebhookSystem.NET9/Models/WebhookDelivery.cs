namespace WebhookSystem.NET9.Models
{
    public class WebhookDelivery
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SubscriptionId { get; set; }
        public Guid EventId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public int HttpStatusCode { get; set; }
        public string? Response { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        // can be remove for hangfire
        public int AttemptNumber { get; set; } = 1;
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }

        // Tiempo que demora la respuesta
        public TimeSpan? ResponseTime { get; set; }

        // Navigation properties
        public WebhookSubscription Subscription { get; set; } = null!;
    }
}
