using System.Data;
using Microsoft.Data.Sqlite;

namespace WebAppMultiTenant;

public interface ITenantDbConnectionFactory
{
    IDbConnection Create();
}

public class TenantDbConnectionFactory : ITenantDbConnectionFactory
{
    private readonly ITenantDatabaseInitializer _initializer;
    private readonly ITenantStore _tenantStore;

    public TenantDbConnectionFactory(ITenantDatabaseInitializer initializer, ITenantStore tenantStore)
    {
        _initializer = initializer;
        _tenantStore = tenantStore;
    }
    // TODO: conexion should be disposable... check proyect master or products
    // where has conexion factory
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