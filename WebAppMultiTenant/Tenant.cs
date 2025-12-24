namespace WebAppMultiTenant;

public class TenantInfo
{
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}

// use store and resolver for get tenant
// IDisposable Change(string name);
public interface ICurrentTenant
{
    bool IsAvailable { get; }
    string? Name { get; }
}

public class CurrentTenant : ICurrentTenant
{
    private readonly ITenantResolver _tenantResolver;
    private readonly ITenantStore _tenantStore;
    public bool IsAvailable => string.IsNullOrEmpty(Name) == false;
    public string? Name => GetCurrentTenant();

    public CurrentTenant(ITenantResolver tenantResolver, ITenantStore tenantStore)
    {
        _tenantResolver = tenantResolver;
        _tenantStore = tenantStore;
    }

    string GetCurrentTenant()
    {
        var tenant = _tenantResolver.ResolveTenantName();
        if (string.IsNullOrEmpty(tenant))
        {
            return string.Empty;
        }

        var tenantInfo = _tenantStore.GetTenant(tenant);
        if (tenantInfo == null)
        {
            return string.Empty;
        }

        return tenantInfo.Name;
    }
}

// TenantConnectionStringResolver

public class TenantResolveResult
{
    public TenantInfo Tenant { get; set; }
}