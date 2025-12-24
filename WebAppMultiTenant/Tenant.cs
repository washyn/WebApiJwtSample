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
    private readonly ITenantStore _tenantStore;
    public bool IsAvailable => string.IsNullOrEmpty(Name) == false;
    public string? Name => GetCurrentTenant();

    public CurrentTenant(ITenantStore tenantStore)
    {
        _tenantStore = tenantStore;
    }

    string GetCurrentTenant()
    {
        var tenantInfo = _tenantStore.GetTenant();
        if (tenantInfo == null)
        {
            return string.Empty;
        }

        return tenantInfo.Name;
    }
}