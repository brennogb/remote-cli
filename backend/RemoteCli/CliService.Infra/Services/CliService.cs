using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CliService.Infra.Services
{
    public class CliService : ServiceBase, IHostLifetime
    {
        private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>();

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        
        private readonly ISignalRService _signalRService;

        public CliService(IHostApplicationLifetime hostApplicationLifetime, ISignalRService signalRService)
        {
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _signalRService = signalRService;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _signalRService.StopAsync(cancellationToken);
            Stop();
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            _hostApplicationLifetime.ApplicationStopping.Register(Stop);

            new Thread(Run).Start();
            return _delayStart.Task;
        }
        
        protected override void OnStart(string[] args)
        {
            _delayStart.TrySetResult(null);
            base.OnStart(args);
        }
        
        protected override void OnStop()
        {
            _hostApplicationLifetime.StopApplication();
            base.OnStop();
        }

        private void Run()
        {
            try
            {
                _signalRService.StartAsync();
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
            }
        }
    }
}