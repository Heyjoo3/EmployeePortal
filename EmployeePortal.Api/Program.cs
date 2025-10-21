using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace EmployeePortal
{
    public class Program
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            // Configure serilog
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();

            Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(configuration)
             .WriteTo.Map("Name", "api", (name, wt) => wt.File($"./Logs/{name}/log-{name}-.txt",
                 rollingInterval: RollingInterval.Day,
                 fileSizeLimitBytes: 1000000,
                 rollOnFileSizeLimit: true,
                 retainedFileCountLimit: 60))
          .CreateLogger();
            CreateHostBuilder(args).Build().Run();
            try
            {
                Log.Information("Starting up...");
                CreateHostBuilder(args).Build().Run();
                Log.Information("Shutting down...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Api host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}



