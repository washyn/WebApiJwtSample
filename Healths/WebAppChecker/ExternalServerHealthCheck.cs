using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebAppChecker;

public class ExternalServerHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public ExternalServerHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5247/health", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External server is healthy.");
            }
            else
            {
                return HealthCheckResult.Unhealthy($"External server responded with status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            return HealthCheckResult.Unhealthy($"Error checking external server: {ex.Message}");
        }
    }
}