namespace WebhookSystem.NET9.Models
{
    public record WebhookPayload(
        string Id,
        string Event,
        string Source,
        DateTime Timestamp,
        string Version,
        object Data,
        Dictionary<string, object>? Metadata = null
    );
}
