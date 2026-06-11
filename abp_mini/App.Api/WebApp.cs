using App.Api.Data;
using App.Api.Entities;
using App.Api.ObjectMapping;
using App.Api.Repositories;
using App.Api.Services;

using Library;
using Library.Application.ObjectMapping;
using Library.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

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


// Register ObjectMapper
        context.Services.AddSingleton<IObjectMapper, DemoObjectMapper>();

// Register Application Services with property injection
        context.Services.AddTransient(provider =>
        {
            var repository = provider.GetRequiredService<IRepository<TodoItem, System.Guid>>();
            var mapper = provider.GetRequiredService<IObjectMapper>();
            var service = new TodoAppService(repository);
            service.ObjectMapper = mapper; // Manual Property Injection
            return service;
        });

        context.Services.AddTransient(provider =>
        {
            var repository = provider.GetRequiredService<IReadOnlyRepository<Category, System.Guid>>();
            var mapper = provider.GetRequiredService<IObjectMapper>();
            var service = new CategoryAppService(repository);
            service.ObjectMapper = mapper;
            return service;
        });

        context.Services.AddTransient<IBookRepository, BookRepository>();

        context.Services.AddTransient(provider =>
        {
            var repository = provider.GetRequiredService<IBookRepository>();
            var mapper = provider.GetRequiredService<IObjectMapper>();
            var service = new BookAppService(repository);
            service.ObjectMapper = mapper;
            return service;
        });

        context.Services.AddTransient(provider =>
        {
            var repository = provider.GetRequiredService<IRepository<Book, System.Guid>>();
            var mapper = provider.GetRequiredService<IObjectMapper>();
            var service = new StudentAppService(repository);
            service.ObjectMapper = mapper;
            return service;
        });
    }
}