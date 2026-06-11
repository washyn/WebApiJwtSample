using App.Api.Data;
using App.Api.Entities;

var builder = WebApplication.CreateBuilder(args);

await builder.Services.AddApplicationAsync<WebApp>();

var app = builder.Build();

// Seed some data for Categories
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Categories.Add(new Category { Id = System.Guid.NewGuid(), Name = "Work" });
    dbContext.Categories.Add(new Category { Id = System.Guid.NewGuid(), Name = "Personal" });
    dbContext.SaveChanges();
}

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
// app.UseAuthorization();
// app.MapControllers();
//
// app.Run();

await app.InitializeApplicationAsync();
await app.RunAsync();