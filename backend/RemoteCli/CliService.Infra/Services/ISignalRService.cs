using System.Threading;
using System.Threading.Tasks;

namespace CliService.Infra.Services
{
    public interface ISignalRService
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}