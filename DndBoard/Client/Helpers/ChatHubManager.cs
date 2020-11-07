using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DndBoard.Client.Helpers
{
    public class ChatHubManager
    {
        private HubConnection _hubConnection;
        private readonly NavigationManager _navigationManager;

        public ChatHubManager(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public async Task StartConnectionAsync()
        {
            _hubConnection = SetupSignalRConnection("/chathub");
            await _hubConnection.StartAsync();
        }

        public async Task CloseConnectionAsync()
        {
            await _hubConnection.DisposeAsync();
        }

        public void SetMessageHandler(Action<string, string> handler)
        {
            _hubConnection.On("ReceiveMessage", handler);
        }

        public async Task SendAsync(string user, string message)
        {
            await _hubConnection.SendAsync("SendMessage", user, message);
        }


        private HubConnection SetupSignalRConnection(string hubUri)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri(hubUri))
                .Build();

            return connection;
        }
    }
}
