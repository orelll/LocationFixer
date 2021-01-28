using System;
using System.Linq;
using Serilog;

namespace JobLocationFixer {
    class Program {
        static void Main (string[] args) {
            PrepareLogger ();
            Log.Information ($@"Application started with arguments: {string.Join(Environment.NewLine, args)}");

            var fileProcessor = new XMLProcessor ();
            try {
                var path = args.Count() > 0 ? args[0] : "";

                fileProcessor.ProcessFile (path);
            } catch (Exception ex) {
                Log.Error ($@"An exception occured: {ex}");
            } finally {
                Log.Information ("Application is going to shutdown");
            }
        }

        private static void PrepareLogger () {
            Log.Logger = new LoggerConfiguration ()
                .MinimumLevel.Debug ()
                .WriteTo.Console ()
                .WriteTo.File ("logfile.log", rollingInterval : RollingInterval.Day)
                .CreateLogger ();
        }
    }
}