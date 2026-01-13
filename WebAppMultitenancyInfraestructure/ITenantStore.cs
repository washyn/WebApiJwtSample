using Microsoft.Extensions.Options;

namespace WebAppMultitenancyInfraestructure;

public interface ITenantStore
{
    Task<TenantConfiguration?> FindAsync(string normalizedName);

    Task<TenantConfiguration?> FindAsync(Guid id);

    Task<IReadOnlyList<TenantConfiguration>> GetListAsync(bool includeDetails = false);
}

public class AppSettingTenantStore : ITenantStore
{
    private readonly ILogger<AppSettingTenantStore> _logger;
    private readonly AbpDefaultTenantStoreOptions _options;

    public AppSettingTenantStore(IOptions<AbpDefaultTenantStoreOptions> options, ILogger<AppSettingTenantStore> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    public Task<TenantConfiguration?> FindAsync(string normalizedName)
    {
        return Task.FromResult(Find(normalizedName));
    }

    public Task<TenantConfiguration?> FindAsync(Guid id)
    {
        return Task.FromResult(Find(id));
    }

    public Task<IReadOnlyList<TenantConfiguration>> GetListAsync(bool includeDetails = false)
    {
        return Task.FromResult<IReadOnlyList<TenantConfiguration>>(_options.Tenants);
    }

    public TenantConfiguration? Find(string normalizedName)
    {
        return _options.Tenants?.FirstOrDefault(t => t.NormalizedName == normalizedName);
    }

    public TenantConfiguration? Find(Guid id)
    {
        return _options.Tenants?.FirstOrDefault(t => t.Id == id);
    }
}