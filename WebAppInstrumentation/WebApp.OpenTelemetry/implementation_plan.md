# Add OpenTelemetry to ASP.NET Core Project

## Goal
Integrate OpenTelemetry for tracing and metrics into the `WebApp.OpenTelemetry` ASP.NET Core project. This includes adding required NuGet packages, configuring OpenTelemetry services, and ensuring telemetry is exported to a console (or other exporter) for verification.

## User Review Required
- **Exporter Choice**: By default we will use the Console exporter for quick verification. If you prefer Jaeger, Zipkin, or OTLP, let us know.
- **Telemetry Scope**: We will instrument ASP.NET Core, HTTP client, and Entity Framework Core. Confirm if additional libraries need instrumentation.

## Open Questions
> [!IMPORTANT]
> - Do you want to keep the existing logging (Serilog) configuration unchanged?
> - Which exporter should be used for production (Console, Jaeger, Zipkin, OTLP)?

## Proposed Changes
---
### Project File
#### [MODIFY] [WebApp.OpenTelemetry.csproj](file:///d:/git-proyects/Poc/WebApp.OpenTelemetry/WebApp.OpenTelemetry.csproj)
Add the following `PackageReference` items inside an `<ItemGroup>`:
```xml
<PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.6.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.6.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.6.0" />
<PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.6.0" />
<PackageReference Include="OpenTelemetry.Instrumentation.EntityFrameworkCore" Version="1.6.0" />
```
---
### Program.cs
#### [MODIFY] [Program.cs](file:///d:/git-proyects/Poc/WebApp.OpenTelemetry/Program.cs)
Insert the OpenTelemetry configuration after building the `WebApplicationBuilder` and before calling `AddApplicationAsync`.
```csharp
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// ... existing code
var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry Tracing
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebApp.OpenTelemetry"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options =>
        {
            // Optional: configure EF Core options
        })
        .AddConsoleExporter(); // Change exporter if needed
});

// OpenTelemetry Metrics (optional but recommended)
builder.Services.AddOpenTelemetryMetrics(metricsBuilder =>
{
    metricsBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebApp.OpenTelemetry"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter();
});

builder.Host.AddAppSettingsSecretsJson()
    .UseAutofac()
    .UseSerilog();
```
---
## Verification Plan
### Automated Tests
- Build the solution with `dotnet build` to ensure the new packages resolve.
- Run the application (`dotnet run` or `dotnet watch run`) and verify console output contains OpenTelemetry traces (e.g., `Activity.Start` lines) for HTTP requests.

### Manual Verification
- Access the web app in a browser (e.g., `https://localhost:5001`).
- Observe console logs for trace information about the incoming request and any EF Core queries.
- If you selected a different exporter, check its UI (Jaeger, Zipkin, etc.) for traces.

---
*Please review the plan, answer the open questions, and approve to proceed with implementation.*
