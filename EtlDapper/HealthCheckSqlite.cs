using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EtlDapper;

public class HealthCheckPostgres : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthCheckPostgres> _logger;

    public HealthCheckPostgres(IConfiguration configuration, ILogger<HealthCheckPostgres> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await CheckSourceConnectionAsync();
        return result
            ? HealthCheckResult.Healthy("Source database connection (Postgres) is OK.")
            : HealthCheckResult.Unhealthy();
    }

    private async Task<bool> CheckSourceConnectionAsync()
    {
        var connectionString = _configuration.GetConnectionString("Source");
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            _logger.LogInformation("Source database connection (Postgres) is OK.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to the source database (Postgres).");
            return false;
        }
    }
}

public class HealthCheckSqlite : IHealthCheck
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthCheckSqlite> _logger;

    public HealthCheckSqlite(IConfiguration configuration, ILogger<HealthCheckSqlite> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<bool> CheckDestinationConnectionAsync()
    {
        var connectionString = _configuration.GetConnectionString("Destination");
        try
        {
            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();
            _logger.LogInformation("Destination database connection (Sqlite) is OK.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to the destination database (Sqlite).");
            return false;
        }
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await CheckDestinationConnectionAsync();
        return result
            ? HealthCheckResult.Healthy("Destination database connection (Sqlite) is OK.")
            : HealthCheckResult.Unhealthy();
    }
}
