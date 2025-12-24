using System.Data;
using Microsoft.Data.Sqlite;

namespace WebAppMultiTenant;

public interface ITenantDbConnectionFactory
{
    IDbConnection Create();
}

public class TenantDbConnectionFactory : ITenantDbConnectionFactory
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITenantDatabaseInitializer _initializer;
    private readonly ITenantStore _tenantStore;

    public TenantDbConnectionFactory(IHttpContextAccessor httpContextAccessor, ITenantDatabaseInitializer initializer,
        ITenantStore tenantStore)
    {
        _httpContextAccessor = httpContextAccessor;
        _initializer = initializer;
        _tenantStore = tenantStore;
    }

    public IDbConnection Create()
    {
        var tenantInfo = _tenantStore.GetTenant() ??
                         throw new InvalidOperationException("Tenant no disponible en el contexto");

        var builder = new SqliteConnectionStringBuilder(tenantInfo.ConnectionString);
        var dataSource = builder.DataSource;
        if (string.IsNullOrWhiteSpace(dataSource))
        {
            throw new InvalidOperationException("ConnectionString inválido para el tenant");
        }

        var dir = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var conn = new SqliteConnection(tenantInfo.ConnectionString);
        conn.Open();
        _initializer.EnsureCreated(tenantInfo.Name, conn);
        return conn;
    }
}