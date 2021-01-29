using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace JobLocationFixer
{
    public class LocationFixerService : IHostedService
    {
        private readonly FixerConfig _config;
        private System.Timers.Timer _timer;
        private bool _stop = false;

        public LocationFixerService(FixerConfig config)
        {
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            CoupleWithTimer();
            return new Task(() => { });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return new Task(() => _stop = true);
        }

        private void CoupleWithTimer()
        {
            _timer = new System.Timers.Timer(_config.IterationDelay); // 10 Seconds
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.Enabled = true;

            PerformScan(_config.FilePath);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (_stop)
                _timer.Enabled = false;
            else

                PerformScan(_config.FilePath);
        }

        private void PerformScan(string filepath)
        {
            try
            {
                var fileProcessor = new XMLProcessor();
                fileProcessor.ProcessFile(filepath);
            }
            catch (Exception ex)
            {
                Log.Error($@"An exception occured: {ex}");
            }
            finally
            {
                Log.Information("Application is going to shutdown");
            }
        }
    }
}
