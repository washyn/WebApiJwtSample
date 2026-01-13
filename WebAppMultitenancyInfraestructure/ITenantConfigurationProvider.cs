namespace WebAppMultitenancyInfraestructure;

public interface ITenantConfigurationProvider
{
    Task<TenantConfiguration?> GetAsync();
}
public class TenantConfigurationProvider : ITenantConfigurationProvider
{
    private readonly ILogger<TenantConfigurationProvider> _logger;
    protected virtual ITenantResolver TenantResolver { get; }
    protected virtual ITenantStore TenantStore { get; }

    public TenantConfigurationProvider(
        ITenantResolver tenantResolver,
        ILogger<TenantConfigurationProvider> logger,
        ITenantStore tenantStore)
    {
        _logger = logger;
        TenantResolver = tenantResolver;
        TenantStore = tenantStore;
    }

    public virtual async Task<TenantConfiguration?> GetAsync()
    {
        var resolveResult = await TenantResolver.ResolveTenantIdOrNameAsync();

        TenantConfiguration? tenant = null;
        if (resolveResult.TenantIdOrName != null)
        {
            tenant = await FindTenantAsync(resolveResult.TenantIdOrName);

            if (tenant == null)
            {
                throw new Exception("Tenant not found");
            }

            if (!tenant.IsActive)
            {
                throw new Exception("Tenant not active");
            }
        }

        return tenant;
    }

    protected virtual async Task<TenantConfiguration?> FindTenantAsync(string tenantIdOrName)
    {
        if (Guid.TryParse(tenantIdOrName, out var parsedTenantId))
        {
            return await TenantStore.FindAsync(parsedTenantId);
        }
        else
        {
            return await TenantStore.FindAsync(tenantIdOrName!);
        }
    }
}
