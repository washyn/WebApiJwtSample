using Volo.Abp.Modularity;

namespace WebAppImpersonation.Pages;

public class AppModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpContextAccessor();
    }
}