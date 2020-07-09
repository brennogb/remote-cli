namespace CliService.Infra.Services
{
    public interface IPowerShellService
    {
        void ExecuteCommand(string sender, string command);
    }
}