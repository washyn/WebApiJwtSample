namespace WebhookSystem.NET9.Models
{
    public class WebhookSubscription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Url { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public List<string> Events { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string? Description { get; set; }

        // can be remove for hangfire
        public int MaxRetries { get; set; } = 3;

        // can be remove for hangfire
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMinutes(1);
        public Dictionary<string, string> Headers { get; set; } = new();

        // Navigation properties
        public List<WebhookDelivery> Deliveries { get; set; } = new();
    }
}
