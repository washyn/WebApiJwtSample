using App.Api.Data;
using App.Api.Entities;
using App.Api.ObjectMapping;
using App.Api.Repositories;
using App.Api.Services;

using Library.Application.ObjectMapping;
using Library.Domain.Repositories;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext (In-Memory for demo)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TodoDemoDb"));

// Register generic repositories from MyLibrary using the specific AppDbContextRepository
builder.Services.AddTransient(typeof(IRepository<,>), typeof(AppDbContextRepository<,>));
builder.Services.AddTransient(typeof(IReadOnlyRepository<,>), typeof(AppDbContextRepository<,>));
builder.Services.AddTransient(typeof(IBasicRepository<,>), typeof(AppDbContextRepository<,>));
builder.Services.AddTransient(typeof(IReadOnlyBasicRepository<,>), typeof(AppDbContextRepository<,>));

// Register ObjectMapper
builder.Services.AddSingleton<IObjectMapper, DemoObjectMapper>();

// Register Application Services with property injection
builder.Services.AddTransient(provider => 
{
    var repository = provider.GetRequiredService<IRepository<TodoItem, System.Guid>>();
    var mapper = provider.GetRequiredService<IObjectMapper>();
    var service = new TodoAppService(repository);
    service.ObjectMapper = mapper; // Manual Property Injection
    return service;
});

builder.Services.AddTransient(provider => 
{
    var repository = provider.GetRequiredService<IReadOnlyRepository<Category, System.Guid>>();
    var mapper = provider.GetRequiredService<IObjectMapper>();
    var service = new CategoryAppService(repository);
    service.ObjectMapper = mapper; 
    return service;
});

builder.Services.AddTransient<IBookRepository, BookRepository>();

builder.Services.AddTransient(provider => 
{
    var repository = provider.GetRequiredService<IBookRepository>();
    var mapper = provider.GetRequiredService<IObjectMapper>();
    var service = new BookAppService(repository);
    service.ObjectMapper = mapper; 
    return service;
});

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
