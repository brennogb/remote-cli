using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CliService.Infra.Services
{
    public class CliHostedService : IHostedService, IDisposable
    {
        private readonly ISignalRService _signalRService;

        public CliHostedService(ISignalRService signalRService)
        {
            _signalRService = signalRService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _signalRService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _signalRService.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}