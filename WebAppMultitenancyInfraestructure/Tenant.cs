using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace WebAppMultitenancyInfraestructure;


public class TenantConfiguration
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string NormalizedName { get; set; } = default!;

    // public ConnectionStrings? ConnectionStrings { get; set; }

    public bool IsActive { get; set; }

    public TenantConfiguration()
    {
        IsActive = true;
    }

    public TenantConfiguration(Guid id, [NotNull] string name)
        : this()
    {
        // Check.NotNull(name, nameof(name));

        Id = id;
        Name = name;
        // ConnectionStrings = new ConnectionStrings();
    }

    public TenantConfiguration(Guid id, [NotNull] string name, [NotNull] string normalizedName)
        : this(id, name)
    {
        // Check.NotNull(normalizedName, nameof(normalizedName));
        NormalizedName = normalizedName;
    }
}



public class AbpDefaultTenantStoreOptions
{
    public TenantConfiguration[] Tenants { get; set; }

    public AbpDefaultTenantStoreOptions()
    {
        Tenants = Array.Empty<TenantConfiguration>();
    }
}

public interface IServiceProviderAccessor
{
    IServiceProvider ServiceProvider { get; }
}

public interface ITenantResolveContext : IServiceProviderAccessor
{
    string? TenantIdOrName { get; set; }

    bool Handled { get; set; }
}

public class TenantResolveContext : ITenantResolveContext
{
    public IServiceProvider ServiceProvider { get; }
    public string? TenantIdOrName { get; set; }
    public bool Handled { get; set; }

    public bool HasResolvedTenantOrHost()
    {
        return Handled || TenantIdOrName != null;
    }

    public TenantResolveContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}

public class AbpTenantResolveOptions
{
    [NotNull] public List<ITenantResolveContributor> TenantResolvers { get; }

    public AbpTenantResolveOptions()
    {
        TenantResolvers = new List<ITenantResolveContributor>();
    }
}

public class TenantResolveResult
{
    public string? TenantIdOrName { get; set; }
    public List<string> AppliedResolvers { get; }

    public TenantResolveResult()
    {
        AppliedResolvers = new List<string>();
    }
}

public interface ICurrentTenant
{
    bool IsAvailable { get; }
    Guid? Id { get; }
    string? Name { get; }
}

public class CurrentTenant : ICurrentTenant
{
    private readonly ITenantConfigurationProvider _configurationProvider;
    private readonly Lazy<TenantConfiguration?> _tenantLazy;

    public bool IsAvailable => Id.HasValue;

    public Guid? Id => _tenantLazy.Value?.Id;

    public string? Name => _tenantLazy.Value?.Name;

    public CurrentTenant(ITenantConfigurationProvider configurationProvider)
    {
        _configurationProvider = configurationProvider;

        _tenantLazy = new Lazy<TenantConfiguration?>(() =>
            _configurationProvider
                .GetAsync()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
        );
    }
}
