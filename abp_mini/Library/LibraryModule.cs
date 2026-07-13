using Lib.Domain.Repositories;
using Lib.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Lib;

[DependsOn(typeof(AbpAutoMapperModule))]
public class LibraryModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Register generic repositories from MyLibrary using the specific AppDbContextRepository
        context.Services.AddTransient(typeof(IRepository<,>), typeof(DbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(DbContextRepository<,>));
        context.Services.AddTransient(typeof(IBasicRepository<,>), typeof(DbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyBasicRepository<,>), typeof(DbContextRepository<,>));
    }
}
// DONE: improve this app service, include same as another lib
// TODO: for register repositories use abp convencional register, ITransiendDependency and IScopedDependency