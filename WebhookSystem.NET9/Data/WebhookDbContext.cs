using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WebhookSystem.NET9.Models;

namespace WebhookSystem.NET9.Data
{
    public class WebhookDbContext : DbContext
    {
        public WebhookDbContext(DbContextOptions<WebhookDbContext> options) : base(options)
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
                entity.Property(e => e.Url).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.Secret).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(1000);

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
                entity.Property(e => e.Url).HasMaxLength(2000).IsRequired();
                // entity.Property(e => e.Payload).HasColumnType("nvarchar(max)");
                // entity.Property(e => e.Payload).HasColumnType("TEXT");
                entity.Property(e => e.Payload);
                // entity.Property(e => e.Response).HasColumnType("nvarchar(max)");
                // entity.Property(e => e.Response).HasColumnType("TEXT");
                entity.Property(e => e.Response);
                entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
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
}
