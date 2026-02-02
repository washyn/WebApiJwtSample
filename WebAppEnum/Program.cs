using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddApplication<AppModule>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapRazorPages();

app.Run();


[DependsOn(typeof(AbpEntityFrameworkCoreSqliteModule))]
public class AppModule : AbpModule
{}