using System.Threading;
using System.Threading.Tasks;
using CliService.Infra.Common.RetryPolicy;
using Microsoft.AspNetCore.SignalR.Client;

namespace CliService.Infra.Services.Support
{
    public class SignalRService : ISignalRService
    {
        private const string URL = "https://localhost:9900/CliServiceHub";
        
        private readonly HubConnection _hubConnection;
        
        private readonly IMachineService _machineService;
        
        private bool _isStarted = false;
        
        public SignalRService(IMachineService machineService)
        {
            _machineService = machineService;
            _hubConnection = new HubConnectionBuilder().WithUrl(URL).Build();
            RegisterListeners(_hubConnection);
        }
        
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            _isStarted = true;
            return RetryPolicy.ExecuteWithRetryAsync(() => _hubConnection.StartAsync(cancellationToken));
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _isStarted = false;
            return RetryPolicy.ExecuteWithRetryAsync(() => _hubConnection.StopAsync(cancellationToken));
        }

        private void RegisterListeners(HubConnection connection)
        {
            connection.On("",
                async () =>
                {
                    await RetryPolicy.ExecuteWithRetryAsync(() => connection.SendAsync(
                            "",
                            _machineService.GetMachineInfo()
                        )
                    );
                });
                
            connection.Closed += (e)
                => _isStarted ? RetryPolicy.ExecuteWithRetryAsync(() => StartAsync(CancellationToken.None)) : Task.CompletedTask;
        }
    }
}