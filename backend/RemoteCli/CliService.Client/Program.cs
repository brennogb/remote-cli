using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using CliService.Infra.Services;
using CliService.Infra.Services.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CliService.Client
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IMachineService, MachineService>();
                    // services.AddSingleton<IPowerShellService, PowerShellService>();
                    services.AddSingleton<ISignalRService, SignalRService>();
                    services.AddHostedService<CliHostedService>();
                });

            if (isService)
            {
                await builder.ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, Infra.Services.CliService>();
                }).Build().StartAsync();
            }
            else
            {
                await builder.RunConsoleAsync();
            }
        }
    }
}