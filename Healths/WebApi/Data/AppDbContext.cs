using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApiLogCollector.Models;

namespace WebApiLogCollector.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HealthProbeRecord> HealthProbeRecords => Set<HealthProbeRecord>();
}

public class DbHc(AppDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("SQLite disponible.")
                : HealthCheckResult.Unhealthy("SQLite no responde.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Error consultando SQLite.", ex);
        }
    }
}
