using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace RegistryApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // packages: serilog, serilog.aspnetcore, serilog.sinks.file
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(new RenderedCompactJsonFormatter(),restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File("logs.txt",rollingInterval: RollingInterval.Day)
                .CreateLogger();
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
