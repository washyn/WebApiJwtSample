using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;

namespace ConsoleAppTenantArquitecture;

class Program
{
    static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    internal static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseEnvironment("Development")
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole(options => { options.FormatterName = ConsoleFormatterNames.Simple; });
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "dd/MM/yyyy HH:mm:ss ";
                });
            })
            .ConfigureServices((hostContext, services) => { services.AddApplication<AbpMultiTenancyModule>(); });
}

public class HostedService : IHostedService
{
    private readonly ILogger<HostedService> _logger;
    private readonly ITenantConfigurationProvider _tenantConfigurationProvider;
    private readonly ICurrentTenant _currentTenant;

    public HostedService(ILogger<HostedService> logger,
        ITenantConfigurationProvider tenantConfigurationProvider,
        ICurrentTenant currentTenant)
    {
        _logger = logger;
        _tenantConfigurationProvider = tenantConfigurationProvider;
        _currentTenant = currentTenant;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting host");
        _logger.LogInformation($"Current tenant id: {_currentTenant.Id}");
        _logger.LogInformation($"Current tenant name: {_currentTenant.Name}");

        using (var tenmant = _currentTenant.Change(Guid.Parse("1a0d0f0a-e0b1-4e3d-b9a4-f3e5c0c3b0e4")))
        {
            _logger.LogInformation($"Current tenant id: {_currentTenant.Id}");
            _logger.LogInformation($"Current tenant name: {_currentTenant.Name}");
        }

        var tenant = await _tenantConfigurationProvider.GetAsync();
        _logger.LogInformation($"Tenant id: {tenant?.Id}, name: {tenant?.Name}");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

public class AsyncLocalCurrentTenantAccessor : ICurrentTenantAccessor
{
    public static AsyncLocalCurrentTenantAccessor Instance { get; } = new();

    public BasicTenantInfo? Current
    {
        get => _currentScope.Value;
        set => _currentScope.Value = value;
    }

    private readonly AsyncLocal<BasicTenantInfo?> _currentScope;

    private AsyncLocalCurrentTenantAccessor()
    {
        _currentScope = new AsyncLocal<BasicTenantInfo?>();
    }
}

public class AbpMultiTenancyModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ICurrentTenantAccessor>(AsyncLocalCurrentTenantAccessor.Instance);

        var configuration = context.Services.GetConfiguration();
        Configure<AbpDefaultTenantStoreOptions>(configuration);

        Configure<AbpTenantResolveOptions>(options =>
        {
            // TODO: cam be add more resolvers for tenant here
            options.TenantResolvers.Insert(0, new CurrentUserTenantResolveContributor());
        });

        context.Services.AddHostedService<HostedService>();
    }
}

public class CurrentUserTenantResolveContributor : ITenantResolveContributor
{
    public const string ContributorName = "CurrentUser";
    public string Name => ContributorName;

    public Task ResolveAsync(ITenantResolveContext context)
    {
        // var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();
        // if (currentUser.IsAuthenticated)
        // {
        //     context.TenantIdOrName = currentUser.GetId();
        // }

        var logger = context.ServiceProvider.GetRequiredService<ILogger<CurrentUserTenantResolveContributor>>();
        logger.LogInformation("CurrentUserTenantResolveContributor.ResolveAsync");
        var id = Guid.Parse("1a0d0f0a-e0b1-4e3d-b9a4-f3e5c0c3b0e4");
        context.Handled = true;
        context.TenantIdOrName = id.ToString();
        return Task.CompletedTask;
    }
}

#region CurrentTenant

// El ICurrentTenant y ICurrentTenantAccessor se usa para acceder representa el tenant
// en el scope actual de la aplicacion
public interface ICurrentTenant
{
    bool IsAvailable { get; }
    Guid? Id { get; }
    string? Name { get; }
    IDisposable Change(Guid? id, string? name = null);
}

public interface ICurrentTenantAccessor
{
    BasicTenantInfo? Current { get; set; }
}

public class BasicTenantInfo
{
    public Guid? TenantId { get; }
    public string? Name { get; }

    public BasicTenantInfo(Guid? tenantId, string? name = null)
    {
        TenantId = tenantId;
        Name = name;
    }
}

#endregion

#region Tenant resolution pipeline

public interface ITenantResolver
{
    [NotNull]
    Task<TenantResolveResult> ResolveTenantIdOrNameAsync();
}

public interface ITenantResolveContributor
{
    string Name { get; }
    Task ResolveAsync(ITenantResolveContext context);
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

#endregion

#region Others

// Shoud be use for get tenant
public interface ITenantConfigurationProvider
{
    Task<TenantConfiguration?> GetAsync(bool saveResolveResult = false);
}

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

// TODO: use app settings store, y el resolver obtiene el id o tenant name de alguna fuente aqui se busca que ese 
// tenant exista en alguna de las fuentes de configuracion
public interface ITenantStore
{
    Task<TenantConfiguration?> FindAsync(string normalizedName);

    Task<TenantConfiguration?> FindAsync(Guid id);

    Task<IReadOnlyList<TenantConfiguration>> GetListAsync(bool includeDetails = false);
}

public class DefaultTenantStore : ITenantStore, ITransientDependency
{
    private readonly ILogger<DefaultTenantStore> _logger;
    private readonly AbpDefaultTenantStoreOptions _options;

    public DefaultTenantStore(IOptions<AbpDefaultTenantStoreOptions> options, ILogger<DefaultTenantStore> logger)
    {
        _logger = logger;
        _options = options.Value;
        _logger.LogInformation("DefaultTenantStore initialized");
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

public class AbpDefaultTenantStoreOptions
{
    public TenantConfiguration[] Tenants { get; set; }

    public AbpDefaultTenantStoreOptions()
    {
        Tenants = Array.Empty<TenantConfiguration>();
    }
}

#endregion

public class TenantResolver : ITenantResolver, ITransientDependency
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
        _logger.LogInformation("TenantResolver initialized");
    }

    public virtual async Task<TenantResolveResult> ResolveTenantIdOrNameAsync()
    {
        _logger.LogInformation("TenantResolver.ResolveTenantIdOrNameAsync");
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

// this is main class for get tenant
public class TenantConfigurationProvider : ITenantConfigurationProvider, ITransientDependency
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
        _logger.LogInformation("TenantConfigurationProvider initialized");
    }

    public virtual async Task<TenantConfiguration?> GetAsync(bool saveResolveResult = false)
    {
        _logger.LogInformation("TenantConfigurationProvider.GetAsync");
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

public class CurrentTenant : ICurrentTenant, ITransientDependency
{
    public virtual bool IsAvailable => Id.HasValue;

    public virtual Guid? Id => _currentTenantAccessor.Current?.TenantId;

    public string? Name => _currentTenantAccessor.Current?.Name;

    private readonly ICurrentTenantAccessor _currentTenantAccessor;

    public CurrentTenant(ICurrentTenantAccessor currentTenantAccessor)
    {
        _currentTenantAccessor = currentTenantAccessor;
    }

    public IDisposable Change(Guid? id, string? name = null)
    {
        return SetCurrent(id, name);
    }

    private IDisposable SetCurrent(Guid? tenantId, string? name = null)
    {
        var parentScope = _currentTenantAccessor.Current;
        _currentTenantAccessor.Current = new BasicTenantInfo(tenantId, name);

        return new DisposeAction<ValueTuple<ICurrentTenantAccessor, BasicTenantInfo?>>(static (state) =>
        {
            var (currentTenantAccessor, parentScope) = state;
            currentTenantAccessor.Current = parentScope;
        }, (_currentTenantAccessor, parentScope));
    }
}