using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using WebApiLogCollector.Models;

namespace WebApiLogCollector.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HealthProbeRecord> HealthProbeRecords => Set<HealthProbeRecord>();
}


// TODO: implement this
public class DbHc : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        
    }
}