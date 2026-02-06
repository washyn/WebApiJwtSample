using System.Security.Cryptography;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sender.Data;
using Sender.Models;

namespace Sender.Controllers;

[ApiController]
[Route("api/suscriptions")]
public class WebHookController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebhookSender _webhookSender;

    public WebHookController(AppDbContext context, IWebhookSender webhookSender)
    {
        _context = context;
        _webhookSender = webhookSender;
    }

    [HttpPost]
    public async Task<WebhookSubscription> CreateSubscriptionAsync(CreateSubscriptionRequest request)
    {
        var subscription = new WebhookSubscription
        {
            Url = request.Url,
            Events = request.Events,
            Description = request.Description,
            Headers = request.Headers ?? new Dictionary<string, string>(),
            Secret = GenerateSecret()
        };
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    [HttpGet]
    public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsAsync()
    {
        return await _context.Subscriptions
            .Where(s => s.IsActive)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<WebhookSubscription?> GetSubscriptionAsync(Guid id)
    {
        return await _context.Subscriptions
            .Include(s => s.Deliveries.OrderByDescending(d => d.AttemptedAt).Take(100))
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    [HttpPut("{id}")]
    public async Task<WebhookSubscription?> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { id });
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
        subscription.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return subscription;
    }

    [HttpDelete("{id}")]
    public async Task<bool> DeleteSubscriptionAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { id });
        if (subscription == null)
            return false;
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
        return true;
    }

    [HttpGet("{subscriptionId}/deliveries")]
    public async Task<IEnumerable<WebhookDelivery>> GetDeliveryHistoryAsync(Guid subscriptionId)
    {
        return await _context.Deliveries
            .Where(d => d.SubscriptionId == subscriptionId)
            .OrderByDescending(d => d.AttemptedAt)
            .Take(1000) // Limit to last 1000 deliveries
            .ToListAsync();
    }

    [HttpPost("{id}/dispatch")]
    public async Task TriggerEventAsync(Guid id)
    {
        var subscription = await _context.Subscriptions.FindAsync(new object[] { id });

        if (subscription == null)
            throw new Exception("Subscription not found");
        // event info
        await _webhookSender.SendWebhookAsync(subscription, new WebhookEvent()
        {
            Data = new { message = "Hello webhook!" },
            EventType = "order.created",
            Source = "OrderService",
            Timestamp = DateTime.UtcNow,
            Version = "1.0",
            Id = Guid.NewGuid(),
            Metadata = new Dictionary<string, object>()
            {
                { "orderId", "12345" }, { "customerId", "67890" }, { "amount", 99.99 }, { "currency", "USD" }
            },
        });
    }

    private static string GenerateSecret()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}

public class CreateSubscriptionRequest
{
    public string Url { get; set; } = string.Empty;
    public List<string> Events { get; set; } = new();
    public string? Description { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
}

public class UpdateSubscriptionRequest
{
    public string? Url { get; set; }
    public List<string>? Events { get; set; }
    public bool? IsActive { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
}