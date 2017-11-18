using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace DotNETCoreDay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddConsole(options => options.IncludeScopes = true);
                    logging.AddDebug();

                    logging.AddFilter<ConsoleLoggerProvider>("DotNETCoreDay.Controllers.HomeController", LogLevel.Information);
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .Build();
    }
}