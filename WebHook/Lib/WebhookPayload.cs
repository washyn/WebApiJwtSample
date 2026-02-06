namespace Lib;

public class WebhookPayload
{
    public string Id { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Version { get; set; } = "1.0";
    public object Data { get; set; } = new();
    public Dictionary<string, object>? Metadata { get; set; }
}

