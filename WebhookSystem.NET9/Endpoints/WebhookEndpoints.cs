using Microsoft.AspNetCore.Mvc;
using WebhookSystem.NET9.Data;
using WebhookSystem.NET9.Models;
using WebhookSystem.NET9.Services;

namespace WebhookSystem.NET9.Endpoints
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/webhooks").WithTags("Webhooks");
            group.MapPost("/subscriptions", CreateSubscription)
                .WithName("CreateWebhookSubscription")
                .WithSummary("Create a new webhook subscription")
                .WithOpenApi()
                .Produces<WebhookSubscription>(201)
                .ProducesValidationProblem()
                .ProducesProblem(500);
            group.MapGet("/subscriptions", GetSubscriptions)
                .WithName("GetWebhookSubscriptions")
                .WithSummary("Get all webhook subscriptions")
                .WithOpenApi()
                .Produces<IEnumerable<WebhookSubscription>>()
                .ProducesProblem(500);
            group.MapGet("/subscriptions/{id:guid}", GetSubscription)
                .WithName("GetWebhookSubscription")
                .WithSummary("Get webhook subscription by ID")
                .WithOpenApi()
                .Produces<WebhookSubscription>()
                .Produces(404)
                .ProducesProblem(500);
            group.MapPut("/subscriptions/{id:guid}", UpdateSubscription)
                .WithName("UpdateWebhookSubscription")
                .WithSummary("Update webhook subscription")
                .WithOpenApi()
                .Produces<WebhookSubscription>()
                .Produces(404)
                .ProducesValidationProblem()
                .ProducesProblem(500);
            group.MapDelete("/subscriptions/{id:guid}", DeleteSubscription)
                .WithName("DeleteWebhookSubscription")
                .WithSummary("Delete webhook subscription")
                .WithOpenApi()
                .Produces(204)
                .Produces(404)
                .ProducesProblem(500);
            group.MapPost("/events", TriggerEvent)
                .WithName("TriggerWebhookEvent")
                .WithSummary("Trigger a webhook event")
                .WithOpenApi()
                .Produces(202)
                .ProducesValidationProblem()
                .ProducesProblem(500);
            group.MapGet("/subscriptions/{id:guid}/deliveries", GetDeliveryHistory)
                .WithName("GetWebhookDeliveryHistory")
                .WithSummary("Get delivery history for a webhook subscription")
                .WithOpenApi()
                .Produces<IEnumerable<WebhookDelivery>>()
                .Produces(404)
                .ProducesProblem(500);
            group.MapPost("/deliveries/{id:guid}/retry", RetryDelivery)
                .WithName("RetryWebhookDelivery")
                .WithSummary("Retry a failed webhook delivery")
                .WithOpenApi()
                .Produces(202)
                .Produces(404)
                .ProducesProblem(500);
            group.MapGet("/subscriptions/{id:guid}/trigger", TrigerSpecificSubscription)
                .WithName("TriggerWebhookEventSpecificSubscription")
                .WithSummary("Trigger a webhook event")
                .WithOpenApi()
                .Produces(202)
                .Produces(404)
                .ProducesProblem(500);
        }

        private static async Task<IResult> CreateSubscription(
            CreateSubscriptionRequest request,
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await webhookService.CreateSubscriptionAsync(request, cancellationToken);
                return Results.Created($"/api/webhooks/subscriptions/{subscription.Id}", subscription);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error creating webhook subscription",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> GetSubscriptions(
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                var subscriptions = await webhookService.GetSubscriptionsAsync(cancellationToken);
                return Results.Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error retrieving webhook subscriptions",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> GetSubscription(
            Guid id,
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await webhookService.GetSubscriptionAsync(id, cancellationToken);
                return subscription != null ? Results.Ok(subscription) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error retrieving webhook subscription",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> UpdateSubscription(
            Guid id,
            UpdateSubscriptionRequest request,
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await webhookService.UpdateSubscriptionAsync(id, request, cancellationToken);
                return subscription != null ? Results.Ok(subscription) : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error updating webhook subscription",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> DeleteSubscription(
            Guid id,
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await webhookService.DeleteSubscriptionAsync(id, cancellationToken);
                return deleted ? Results.NoContent() : Results.NotFound();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error deleting webhook subscription",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> TrigerSpecificSubscription(
            [FromRoute] Guid id,
            [FromServices] IWebhookService webhookService,
            [FromServices] WebhookDbContext context,
            [FromServices] IWebhookSender webhookSender,
            CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await webhookService.GetSubscriptionAsync(id, cancellationToken);
                if (subscription == null)
                {
                    return Results.NotFound();
                }

                // send webhook, puede que el otro este tronando porque no tiene eventos hijos
                await webhookSender.SendWebhookAsync(subscription, new WebhookEvent()
                {
                    Data = new { message = "Hello webhook!" },
                    EventType = "order.created",
                    Source = "OrderService",
                    Timestamp = DateTime.UtcNow,
                    Version = "1.0",
                    Id = Guid.NewGuid(),
                    Metadata = new Dictionary<string, object>()
                    {
                        { "orderId", "12345" },
                        { "customerId", "67890" },
                        { "amount", 99.99 },
                        { "currency", "USD" }
                    },
                }, cancellationToken);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error retrieving webhook event",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }


        private static async Task<IResult> TriggerEvent(
            WebhookEvent webhookEvent,
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                await webhookService.TriggerEventAsync(webhookEvent, cancellationToken);
                return Results.Accepted();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error triggering webhook event",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> GetDeliveryHistory(
            Guid id,
            IWebhookService webhookService,
            CancellationToken cancellationToken)
        {
            try
            {
                var deliveries = await webhookService.GetDeliveryHistoryAsync(id, cancellationToken);
                return Results.Ok(deliveries);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error retrieving delivery history",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }

        private static async Task<IResult> RetryDelivery(
            Guid id,
            IWebhookSender webhookSender,
            CancellationToken cancellationToken)
        {
            try
            {
                await webhookSender.RetryFailedWebhookAsync(id, cancellationToken);
                return Results.Accepted();
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "Error retrying webhook delivery",
                    detail: ex.Message,
                    statusCode: 500);
            }
        }
    }
}
