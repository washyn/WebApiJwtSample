using Serilog;
using Serilog.Events;

namespace Agent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.log"))
                .WriteTo.Async(c => c.Console())
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.MapControllers();

            app.Run();
        }
    }
}