namespace WebAppMultitenancyInfraestructure;

public interface ITenantResolveContributor
{
    string Name { get; }
    Task ResolveAsync(ITenantResolveContext context);
}

public class HttpHeaderTenantResolveContributor : ITenantResolveContributor
{
    public const string ContributorName = "HttpHeader";
    public string Name => ContributorName;

    public Task ResolveAsync(ITenantResolveContext context)
    {
        var httpContext = context.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
        if (httpContext.HttpContext != null)
        {
            var value = httpContext.HttpContext.Request.Headers["tenant-name"].FirstOrDefault();
            if (!string.IsNullOrEmpty(value))
            {
                context.Handled = true;
                context.TenantIdOrName = value;
            }
        }

        return Task.CompletedTask;
    }
}