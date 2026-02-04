// Program.cs

using Hangfire;
using Hangfire.SQLite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebhookSystem.NET9;
using WebhookSystem.NET9.Data;
using WebhookSystem.NET9.Endpoints;
using WebhookSystem.NET9.Middleware;
using WebhookSystem.NET9.Services;



var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "Webhook System API", Version = "v1" }); });

// Entity Framework
builder.Services.AddDbContext<WebhookDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetSqlite()));

// HttpClient with policies
builder.Services.AddHttpClient<IWebhookSender, WebhookSender>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.Add("User-Agent", "WebhookSystem.NET9/1.0");
    })
    .AddStandardResilienceHandler(); // .NET 9 resilience patterns

// Webhook services
builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddScoped<IWebhookSender, WebhookSender>();
builder.Services.AddSingleton<IHmacAuthenticationService, HmacAuthenticationService>();

// Background job processing with Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetSqlite()));
builder.Services.AddHangfireServer(options => { options.WorkerCount = Environment.ProcessorCount * 2; });

// Health checks
builder.Services.AddHealthChecks()
    .AddSqlite(
        connectionString: builder.Configuration.GetSqlite(),
        name: "database"
    )
    .AddHangfire(options => options.MinimumAvailableServers = 1);

// CORS for development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    });
}


var app = builder.Build();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors();
}

// Add custom middleware
app.UseSerilogRequestLogging();
app.UseMiddleware<WebhookAuthenticationMiddleware>();
app.UseHttpsRedirection();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = app.Environment.IsDevelopment()
        ? new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
        : Array.Empty<Hangfire.Dashboard.IDashboardAuthorizationFilter>()
});

// Map endpoints
app.MapWebhookEndpoints();
app.MapWebhookReceiverEndpoints();
// Health check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.TotalMilliseconds
            }),
            timestamp = DateTime.UtcNow
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});
// Apply migrations in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<WebhookDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();