using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public abstract class AdminHubCommand
    {
        public abstract string Name { get; }

        protected HubConnection _hubConnection;

        public virtual async Task ExecuteAsync()=>
            await _hubConnection?.InvokeAsync(Name);
        internal void SetHubConnection(HubConnection hubConnection) => 
            _hubConnection = hubConnection;
    }
}
