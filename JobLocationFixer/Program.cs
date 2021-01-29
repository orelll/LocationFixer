using System;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JobLocationFixer {
    class Program {
        static void Main (string[] args) {
            PrepareLogger ();
            Log.Information ($@"Application started with arguments: {string.Join(Environment.NewLine, args)}");

            var hostBuilder = CreateHostBuilder (args);
            
            try {

                hostBuilder.Build ().Run ();
            } catch (Exception ex) {
                Log.Error ($"An error occured: {ex}");
            }
        }

        public static IHostBuilder CreateHostBuilder (string[] args) =>
            Host.CreateDefaultBuilder (args)
            .ConfigureServices ((hostContext, services) => {
                var filepath = args.Count () > 0 ? args[0] : "";
                services.AddSingleton (new FixerConfig {
                    FilePath = filepath,
                    IterationDelay = Convert.ToInt32(TimeSpan.FromHours(8).TotalMilliseconds)
                });
                services.AddHostedService<LocationFixerService> ();
            }).UseWindowsService ();

        private static void PrepareLogger () {
            Log.Logger = new LoggerConfiguration ()
                .MinimumLevel.Debug ()
                .WriteTo.Console ()
                .WriteTo.File ("logfile.log", rollingInterval : RollingInterval.Day)
                .CreateLogger ();
        }
    }
}