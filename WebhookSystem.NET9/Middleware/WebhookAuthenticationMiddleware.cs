using System.Text;
using WebhookSystem.NET9.Services;

namespace WebhookSystem.NET9.Middleware
{
    public class WebhookAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHmacAuthenticationService _hmacService;
        private readonly ILogger<WebhookAuthenticationMiddleware> _logger;
        public WebhookAuthenticationMiddleware(
            RequestDelegate next,
            IHmacAuthenticationService hmacService,
            ILogger<WebhookAuthenticationMiddleware> logger)
        {
            _next = next;
            _hmacService = hmacService;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply to webhook endpoints
            if (!context.Request.Path.StartsWithSegments("/webhooks"))
            {
                await _next(context);
                return;
            }
            // Skip authentication for GET requests (health checks, etc.)
            if (context.Request.Method == HttpMethods.Get)
            {
                await _next(context);
                return;
            }
            try
            {
                // Enable request body buffering for multiple reads
                context.Request.EnableBuffering();
                // Read the request body
                var body = await ReadRequestBodyAsync(context.Request);
                // Extract authentication headers
                var signature = ExtractSignature(context.Request.Headers);
                var timestamp = ExtractTimestamp(context.Request.Headers);
                var secret = ExtractSecret(context.Request.Headers);
                if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secret))
                {
                    _logger.LogWarning("Missing required authentication headers");
                    await WriteUnauthorizedResponse(context);
                    return;
                }
                // Validate timestamp if provided
                if (!string.IsNullOrEmpty(timestamp))
                {
                    if (!_hmacService.ValidateTimestamp(timestamp))
                    {
                        _logger.LogWarning("Invalid or expired timestamp: {Timestamp}", timestamp);
                        await WriteUnauthorizedResponse(context);
                        return;
                    }
                }
                // Validate HMAC signature
                var isValid = _hmacService.ValidateSignature(body, signature, secret);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid HMAC signature for webhook request");
                    await WriteUnauthorizedResponse(context);
                    return;
                }
                // Reset body position for downstream middleware
                context.Request.Body.Position = 0;
                // Authentication successful, continue pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during webhook authentication");
                await WriteErrorResponse(context);
            }
        }
        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
        private static string? ExtractSignature(IHeaderDictionary headers)
        {
            // Support multiple signature header formats
            return headers.TryGetValue("X-Webhook-Signature", out var signature) ? signature.ToString() :
                   headers.TryGetValue("X-Hub-Signature-256", out var githubSig) ? githubSig.ToString() :
                   headers.TryGetValue("Authorization", out var auth) ? auth.ToString() :
                   null;
        }
        private static string? ExtractTimestamp(IHeaderDictionary headers)
        {
            return headers.TryGetValue("X-Webhook-Timestamp", out var timestamp) ? timestamp.ToString() : null;
        }
        private static string? ExtractSecret(IHeaderDictionary headers)
        {
            return headers.TryGetValue("X-API-Key", out var apiKey) ? apiKey.ToString() :
                   headers.TryGetValue("X-Webhook-Secret", out var secret) ? secret.ToString() :
                   null;
        }
        private static async Task WriteUnauthorizedResponse(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var response = new { error = "Unauthorized", message = "Invalid webhook authentication" };
            await context.Response.WriteAsJsonAsync(response);
        }
        private static async Task WriteErrorResponse(HttpContext context)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var response = new { error = "Internal Server Error", message = "Webhook authentication error" };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
