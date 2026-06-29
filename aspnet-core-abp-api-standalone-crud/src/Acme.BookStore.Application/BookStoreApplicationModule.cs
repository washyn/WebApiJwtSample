using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace Acme.BookStore;

[DependsOn(
    typeof(BookStoreDomainModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpObjectExtendingModule)
)]
public class BookStoreApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<BookStoreApplicationModule>();
        });
    }
}
