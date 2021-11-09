using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class GetProcessingInfoCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public string TargetId { get; private set; }
        public override string Name => "GetProcessingInfo";

        public GetProcessingInfoCommand(string targetId)
        {
            TargetId = targetId;
        }

        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId);
    }
}
