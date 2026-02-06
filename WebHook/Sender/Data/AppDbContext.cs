using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using Sender.Models;

namespace Sender.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<WebhookSubscription> Subscriptions { get; set; }
    public DbSet<WebhookDelivery> Deliveries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // WebhookSubscription configuration
        modelBuilder.Entity<WebhookSubscription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Secret);
            entity.Property(e => e.Description);

            // JSON conversion for complex properties
            entity.Property(e => e.Events)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ??
                         new List<string>());
            entity.Property(e => e.Headers)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ??
                         new Dictionary<string, string>());
            entity.HasIndex(e => e.Url);
            entity.HasIndex(e => e.IsActive);
        });
        // WebhookDelivery configuration
        modelBuilder.Entity<WebhookDelivery>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Payload);
            entity.Property(e => e.Response);
            entity.Property(e => e.ErrorMessage);
            entity.HasOne(d => d.Subscription)
                .WithMany(s => s.Deliveries)
                .HasForeignKey(d => d.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.SubscriptionId);
            entity.HasIndex(e => e.AttemptedAt);
            entity.HasIndex(e => e.IsSuccessful);
        });
        base.OnModelCreating(modelBuilder);
    }
}