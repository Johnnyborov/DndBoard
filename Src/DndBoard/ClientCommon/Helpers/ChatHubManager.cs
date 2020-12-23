using System;
using System.Threading.Tasks;
using DndBoard.Shared;
using DndBoard.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace DndBoard.ClientCommon.Helpers
{
    public class ChatHubManager
    {
        private string _boardId;
        private HubConnection _hubConnection;
        private readonly NavigationManager _navigationManager;

        public ChatHubManager(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void SetupConnectionAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri(BoardsHubContract.BaseAddress))
                .Build();
        }

        public async Task StartConnectionAsync() =>
            await _hubConnection.StartAsync();

        public async Task CloseConnectionAsync() =>
            await _hubConnection.DisposeAsync();


        public async Task RequestAllModelsAsync() =>
            await _hubConnection.SendAsync(BoardsHubContract.RequestAllModels, _boardId);

        public async Task RequestAllCoordsAsync() =>
            await _hubConnection.SendAsync(BoardsHubContract.RequestAllCoords, _boardId);


        public async Task AddModelsAsync(UploadedFiles uploadedFiles) =>
            await _hubConnection.SendAsync(BoardsHubContract.AddModels, uploadedFiles);

        public void SetModelsAddedHandler(Func<UploadedFiles, Task> handler) =>
            _hubConnection.On(BoardsHubContract.ModelsAdded, handler);


        public async Task DeleteModelAsync(string modelId) =>
            await _hubConnection.SendAsync(BoardsHubContract.DeleteModel, _boardId, modelId);

        public void SetModelDeletedHandler(Action<string> handler) =>
            _hubConnection.On(BoardsHubContract.ModelDeleted, handler);


        public async Task SendCoordsAsync(string coordsChangeDataJson) =>
            await _hubConnection.SendAsync(BoardsHubContract.CoordsChanged, _boardId, coordsChangeDataJson);

        public void SetCoordsReceivedHandler(Action<string> handler) =>
            _hubConnection.On(BoardsHubContract.CoordsChanged, handler);


        public async Task SendIconInstanceRemovedAsync(string instanceId) =>
            await _hubConnection.SendAsync(BoardsHubContract.IconInstanceRemoved, _boardId, instanceId);

        public void SetIconInstanceRemovedHandler(Action<string> handler) =>
            _hubConnection.On(BoardsHubContract.IconInstanceRemoved, handler);


        public async Task ConnectAsync(string boardId)
        {
            _boardId = boardId;
            await _hubConnection.SendAsync(BoardsHubContract.Connect, _boardId);
        }

        public void SetConnectedHandler(Func<string, Task> handler) =>
            _hubConnection.On(BoardsHubContract.Connected, handler);
    }
}
