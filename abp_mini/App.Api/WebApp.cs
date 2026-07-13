using App.Api.Data;
using App.Api.Entities;
using App.Api.ObjectMapping;
using App.Api.Repositories;
using App.Api.Services;

using Library;
using Library.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.AutoMapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;

[DependsOn(typeof(LibraryModule))]
public class WebApp : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Add services to the container.
        context.Services.AddControllers();
        context.Services.AddEndpointsApiExplorer();
        context.Services.AddSwaggerGen();

// Register DbContext (In-Memory for demo)
        context.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("TodoDemoDb"));
        context.Services.AddTransient(typeof(IRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IBasicRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyBasicRepository<,>), typeof(AppDbContextRepository<,>));


// Configure AutoMapper
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WebApp>();
        });

        context.Services.AddTransient<IBookRepository, BookRepository>();
        context.Services.AddTransient(provider =>
        {
            var service = new TodoAppService(provider.GetRequiredService<IRepository<TodoItem, Guid>>())
            {
                LazyServiceProvider = provider.GetRequiredService<IAbpLazyServiceProvider>()
            };
            return service;
        });
        context.Services.AddTransient(provider =>
        {
            var service = new CategoryAppService(provider.GetRequiredService<IReadOnlyRepository<Category, Guid>>())
            {
                LazyServiceProvider = provider.GetRequiredService<IAbpLazyServiceProvider>()
            };
            return service;
        });
        context.Services.AddTransient(provider =>
        {
            var service = new BookAppService(provider.GetRequiredService<IBookRepository>())
            {
                LazyServiceProvider = provider.GetRequiredService<IAbpLazyServiceProvider>()
            };
            return service;
        });
        context.Services.AddTransient(provider =>
        {
            var service = new StudentAppService(provider.GetRequiredService<IRepository<Book, Guid>>())
            {
                LazyServiceProvider = provider.GetRequiredService<IAbpLazyServiceProvider>()
            };
            return service;
        });
    }
}
// TODO: test lazy service provuder for mapper
// integrar con el library