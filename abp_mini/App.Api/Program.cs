using App.Api.Data;
using App.Api.Entities;
using App.Api.ObjectMapping;
using App.Api.Repositories;
using App.Api.Services;

using Library.Application.ObjectMapping;
using Library.Domain.Repositories;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Modularity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication<WebApp>();

var app = builder.Build();

// Seed some data for Categories
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Categories.Add(new Category { Id = System.Guid.NewGuid(), Name = "Work" });
    dbContext.Categories.Add(new Category { Id = System.Guid.NewGuid(), Name = "Personal" });
    dbContext.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

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

// Register generic repositories from MyLibrary using the specific AppDbContextRepository
        context.Services.AddTransient(typeof(IRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IBasicRepository<,>), typeof(AppDbContextRepository<,>));
        context.Services.AddTransient(typeof(IReadOnlyBasicRepository<,>), typeof(AppDbContextRepository<,>));

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
