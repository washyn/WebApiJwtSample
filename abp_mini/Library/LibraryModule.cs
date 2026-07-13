using Library.Application.ObjectMapping;
using Library.Domain.Repositories;
using Library.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Library;

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
        context.Services.AddTransient<IObjectMapper, AutoMapperObjectMapper>();
        context.Services.AddTransient(typeof(IObjectMapper<>), typeof(AutoMapperObjectMapper<>));
    }
}
// DONE: improve this app service, include same as another lib
// TODO: for register repositories use abp convencional register, ITransiendDependency and IScopedDependency