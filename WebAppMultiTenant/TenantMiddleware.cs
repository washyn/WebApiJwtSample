using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebAppMultiTenant;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantResolver _tenantResolver;
    private readonly ITenantStore _tenantStore;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next,
        ITenantResolver tenantResolver,
        ITenantStore tenantStore,
        ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _tenantResolver = tenantResolver;
        _tenantStore = tenantStore;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantName = _tenantResolver.ResolveTenantName();
        if (string.IsNullOrWhiteSpace(tenantName))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Header x-tenant requerido");
            return;
        }

        var tenantInfo = _tenantStore.GetTenant();
        if (tenantInfo is null)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("Inquilino no encontrado");
            return;
        }

        using (_logger.BeginScope(new Dictionary<string, object>
                   { { "Tenant", tenantName } }))
        {
            await _next(context);
        }
    }
}