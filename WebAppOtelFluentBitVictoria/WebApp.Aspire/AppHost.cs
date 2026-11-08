var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApp_OpenTelemetry>("webappaspire");

builder.Build().Run();
