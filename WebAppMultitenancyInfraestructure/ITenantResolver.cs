using Microsoft.Extensions.Options;

namespace WebAppMultitenancyInfraestructure;

public interface ITenantResolver
{
    Task<TenantResolveResult> ResolveTenantIdOrNameAsync();
}
public class TenantResolver : ITenantResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantResolver> _logger;
    private readonly AbpTenantResolveOptions _options;

    public TenantResolver(IOptions<AbpTenantResolveOptions> options, IServiceProvider serviceProvider,
        ILogger<TenantResolver> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    public virtual async Task<TenantResolveResult> ResolveTenantIdOrNameAsync()
    {
        var result = new TenantResolveResult();

        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new TenantResolveContext(serviceScope.ServiceProvider);

            foreach (var tenantResolver in _options.TenantResolvers)
            {
                await tenantResolver.ResolveAsync(context);

                result.AppliedResolvers.Add(tenantResolver.Name);

                if (context.HasResolvedTenantOrHost())
                {
                    result.TenantIdOrName = context.TenantIdOrName;
                    break;
                }
            }
        }

        return result;
    }
}