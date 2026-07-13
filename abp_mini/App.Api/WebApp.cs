using App.Api.Data;
using App.Api.Entities;
using App.Api.ObjectMapping;
using App.Api.Repositories;
using App.Api.Services;

using Library;
using Library.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;

[DependsOn(typeof(AbpAutofacModule))]
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

        context.Services.AddTransient<IBookRepository, BookRepository>();
        
        context.Services.AddAutoMapperObjectMapper<WebApp>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            /* Uncomment `validate: true` if you want to enable the Configuration Validation feature.
             * See AutoMapper's documentation to learn what it is:
             * https://docs.automapper.org/en/stable/Configuration-validation.html
             */
            options.AddMaps<WebApp>( /* validate: true */);
        });
    }
}