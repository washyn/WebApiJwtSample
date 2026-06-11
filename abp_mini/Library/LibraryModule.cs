using App.Api.Data;

using Library.Domain.Repositories;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;

namespace Library;

public class LibraryModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Register generic repositories from MyLibrary using the specific AppDbContextRepository
        context.Services.AddTransient(typeof(IRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IBasicRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyBasicRepository<,>), typeof(AppDbContextRepository<,>));
    }
}