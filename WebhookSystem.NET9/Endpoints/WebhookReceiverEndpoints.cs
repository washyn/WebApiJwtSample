using System.Text.Json;
using WebhookSystem.NET9.Services;

namespace WebhookSystem.NET9.Endpoints
{
    public static class WebhookReceiverEndpoints
    {
        public static void MapWebhookReceiverEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/webhooks/receive").WithTags("Webhook Receivers");
            // Generic webhook receiver
            group.MapPost("/generic", ReceiveGenericWebhook)
                .WithName("ReceiveGenericWebhook")
                .WithSummary("Receive generic webhook payload")
                .WithOpenApi()
                .Produces(200)
                .Produces(400)
                .Produces(401);
            // GitHub webhook receiver
            group.MapPost("/github", ReceiveGitHubWebhook)
                .WithName("ReceiveGitHubWebhook")
                .WithSummary("Receive GitHub webhook payload")
                .WithOpenApi()
                .Produces(200)
                .Produces(400)
                .Produces(401);
            // Stripe webhook receiver
            group.MapPost("/stripe", ReceiveStripeWebhook)
                .WithName("ReceiveStripeWebhook")
                .WithSummary("Receive Stripe webhook payload")
                .WithOpenApi()
                .Produces(200)
                .Produces(400)
                .Produces(401);
            // Health check endpoint
            group.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
                .WithName("WebhookReceiverHealth")
                .WithSummary("Health check for webhook receivers")
                .WithOpenApi();
        }
        private static async Task<IResult> ReceiveGenericWebhook(
            HttpContext context,
            ILogger<Program> logger)
        {
            try
            {
                // Read the request body
                using var reader = new StreamReader(context.Request.Body);
                var payload = await reader.ReadToEndAsync();
                // Extract headers
                var headers = context.Request.Headers.ToDictionary(
                    h => h.Key,
                    h => string.Join(",", h.Value.ToArray())
                );
                // Log the webhook
                logger.LogInformation("Received generic webhook: {Payload}", payload);
                logger.LogDebug("Webhook headers: {@Headers}", headers);
                // Process the webhook (implement your business logic here)
                await ProcessGenericWebhook(payload, headers, logger);
                return Results.Ok(new
                {
                    message = "Webhook received successfully",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing generic webhook");
                return Results.BadRequest(new { error = "Failed to process webhook" });
            }
        }
        private static async Task<IResult> ReceiveGitHubWebhook(
            HttpContext context,
            IHmacAuthenticationService hmacService,
            ILogger<Program> logger)
        {
            try
            {
                // Read the request body
                using var reader = new StreamReader(context.Request.Body);
                var payload = await reader.ReadToEndAsync();
                // Extract GitHub-specific headers
                var eventType = context.Request.Headers["X-GitHub-Event"].FirstOrDefault();
                var signature = context.Request.Headers["X-Hub-Signature-256"].FirstOrDefault();
                var deliveryId = context.Request.Headers["X-GitHub-Delivery"].FirstOrDefault();
                if (string.IsNullOrEmpty(signature))
                {
                    logger.LogWarning("GitHub webhook missing signature");
                    return Results.Unauthorized();
                }
                // Validate signature (you would get the secret from configuration)
                var secret = "your-github-webhook-secret"; // Get from configuration
                if (!hmacService.ValidateSignature(payload, signature, secret))
                {
                    logger.LogWarning("Invalid GitHub webhook signature");
                    return Results.Unauthorized();
                }
                logger.LogInformation("Received GitHub webhook - Event: {EventType}, Delivery: {DeliveryId}",
                    eventType, deliveryId);
                // Process GitHub webhook
                await ProcessGitHubWebhook(eventType!, payload, logger);
                return Results.Ok(new
                {
                    message = "GitHub webhook processed successfully",
                    eventType,
                    deliveryId
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing GitHub webhook");
                return Results.BadRequest(new { error = "Failed to process GitHub webhook" });
            }
        }
        private static async Task<IResult> ReceiveStripeWebhook(
            HttpContext context,
            IHmacAuthenticationService hmacService,
            ILogger<Program> logger)
        {
            try
            {
                using var reader = new StreamReader(context.Request.Body);
                var payload = await reader.ReadToEndAsync();
                // Extract Stripe-specific headers
                var signature = context.Request.Headers["Stripe-Signature"].FirstOrDefault();
                if (string.IsNullOrEmpty(signature))
                {
                    logger.LogWarning("Stripe webhook missing signature");
                    return Results.Unauthorized();
                }
                // Parse Stripe signature format: t=timestamp,v1=signature
                var signatureParts = signature.Split(',')
                    .Select(part => part.Split('='))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(parts => parts[0], parts => parts[1]);
                if (!signatureParts.TryGetValue("t", out var timestamp) ||
                    !signatureParts.TryGetValue("v1", out var webhookSignature))
                {
                    logger.LogWarning("Invalid Stripe webhook signature format");
                    return Results.Unauthorized();
                }
                // Validate timestamp (Stripe recommends checking within 5 minutes)
                if (!hmacService.ValidateTimestamp(timestamp, TimeSpan.FromMinutes(5)))
                {
                    logger.LogWarning("Stripe webhook timestamp too old or invalid");
                    return Results.Unauthorized();
                }
                // Validate signature
                var secret = "your-stripe-webhook-secret"; // Get from configuration
                var signedPayload = $"{timestamp}.{payload}";
                if (!hmacService.ValidateSignature(signedPayload, webhookSignature, secret))
                {
                    logger.LogWarning("Invalid Stripe webhook signature");
                    return Results.Unauthorized();
                }
                // Parse Stripe event
                var stripeEvent = JsonSerializer.Deserialize<JsonElement>(payload);
                var eventType = stripeEvent.GetProperty("type").GetString();
                logger.LogInformation("Received Stripe webhook - Event: {EventType}", eventType);
                // Process Stripe webhook
                await ProcessStripeWebhook(eventType!, payload, logger);
                return Results.Ok(new
                {
                    message = "Stripe webhook processed successfully",
                    eventType
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Stripe webhook");
                return Results.BadRequest(new { error = "Failed to process Stripe webhook" });
            }
        }
        private static async Task ProcessGenericWebhook(string payload, Dictionary<string, string> headers, ILogger logger)
        {
            // Implement your generic webhook processing logic
            logger.LogInformation("Processing generic webhook with {HeaderCount} headers", headers.Count);

            // Example: Parse JSON and process
            try
            {
                var jsonData = JsonSerializer.Deserialize<JsonElement>(payload);
                // Process the data as needed
            }
            catch (JsonException)
            {
                logger.LogInformation("Non-JSON payload received, processing as text");
            }
            await Task.Delay(100); // Simulate processing time
        }
        private static async Task ProcessGitHubWebhook(string eventType, string payload, ILogger logger)
        {
            logger.LogInformation("Processing GitHub {EventType} event", eventType);
            switch (eventType)
            {
                case "push":
                    // Handle push events
                    logger.LogInformation("Processing GitHub push event");
                    break;
                case "pull_request":
                    // Handle pull request events
                    logger.LogInformation("Processing GitHub pull request event");
                    break;
                case "issues":
                    // Handle issue events
                    logger.LogInformation("Processing GitHub issues event");
                    break;
                default:
                    logger.LogInformation("Unhandled GitHub event type: {EventType}", eventType);
                    break;
            }
            await Task.Delay(100); // Simulate processing time
        }
        private static async Task ProcessStripeWebhook(string eventType, string payload, ILogger logger)
        {
            logger.LogInformation("Processing Stripe {EventType} event", eventType);
            switch (eventType)
            {
                case "payment_intent.succeeded":
                    // Handle successful payments
                    logger.LogInformation("Processing successful payment");
                    break;
                case "payment_intent.payment_failed":
                    // Handle failed payments
                    logger.LogInformation("Processing failed payment");
                    break;
                case "customer.subscription.created":
                    // Handle new subscriptions
                    logger.LogInformation("Processing new subscription");
                    break;
                default:
                    logger.LogInformation("Unhandled Stripe event type: {EventType}", eventType);
                    break;
            }
            await Task.Delay(100); // Simulate processing time
        }
    }
}
