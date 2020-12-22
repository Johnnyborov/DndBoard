using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DndBoard.ClientCommon.Helpers;
using DndBoard.ClientCommon.Models;
using DndBoard.ClientCommon.Store;
using Microsoft.AspNetCore.Components;

namespace DndBoard.ClientCommon.Components
{
    public partial class BoardComponent : ComponentBase, IAsyncDisposable
    {
        private string _boardId;
        private string _connectedBoardId;
        [Inject] private ChatHubManager _chatHubManager { get; set; }
        [Inject] private AppState _appState { get; set; }


        public async ValueTask DisposeAsync()
        {
            await _chatHubManager.CloseConnectionAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            _appState.ChatHubManager = _chatHubManager;
            _chatHubManager.SetupConnectionAsync();
            _chatHubManager.SetConnectedHandler(ConnectedHanlder);
            await _chatHubManager.StartConnectionAsync();
        }

        private async Task ConnectAsync()
        {
            await _chatHubManager.ConnectAsync(_boardId);
        }

        private async Task ConnectedHanlder(string boardId)
        {
            _appState.IconsInstances = new List<DndIconElem>();
            _appState.IconsModels = new List<DndIconElem>();
            _connectedBoardId = boardId;
            StateHasChanged();
            await _appState.InvokeBoardIdChanged(boardId);
        }
    }
}
