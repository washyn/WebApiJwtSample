namespace WebAppMultiTenant;

public interface ITenantResolver
{
    string ResolveTenantName();
}

public class HeaderTenantResolver : ITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public string ResolveTenantName()
    {
        var tenantName = _httpContextAccessor.HttpContext?.Request.Headers["x-tenant"].FirstOrDefault();
        if (string.IsNullOrEmpty(tenantName))
        {
            return string.Empty;
        }

        return tenantName;
    }
}

public interface ITenantStore
{
    TenantInfo? GetTenant();
}

public class TenantStoreInMemory : ITenantStore
{
    private readonly ITenantResolver _tenantResolver;
    private readonly Dictionary<string, TenantInfo> _tenants = new();

    public TenantStoreInMemory(ITenantResolver tenantResolver)
    {
        _tenantResolver = tenantResolver;
        _tenants.Add("tenantA", new TenantInfo()
        {
            Name = "tenantA",
            ConnectionString = "Data Source=Data/tenantA.db"
        });
        _tenants.Add("tenantB", new TenantInfo()
        {
            Name = "tenantB",
            ConnectionString = "Data Source=Data/tenantB.db"
        });
    }

    public TenantInfo? GetTenant()
    {
        var name = _tenantResolver.ResolveTenantName();
        return _tenants.TryGetValue(name, out var info) ? info : null;
    }
}