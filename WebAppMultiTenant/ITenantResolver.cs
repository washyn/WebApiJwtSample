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
    TenantInfo? GetTenant(string name);
}

public class TenantStoreInMemory : ITenantStore
{
    private readonly Dictionary<string, TenantInfo> _tenants = new();

    public TenantStoreInMemory()
    {
        _tenants.Add("tenantA", new TenantInfo()
        {
            Name = "tenantA",
            ConnectionString = "Server=localhost;Database=tenantA;User Id=sa;Password=12345678;"
        });
        _tenants.Add("tenantB", new TenantInfo()
        {
            Name = "tenantB",
            ConnectionString = "Server=localhost;Database=tenantB;User Id=sa;Password=12345678;"
        });
    }

    public TenantInfo? GetTenant(string name)
    {
        return _tenants.TryGetValue(name, out var info) ? info : null;
    }
}