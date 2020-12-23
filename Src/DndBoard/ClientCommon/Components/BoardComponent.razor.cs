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
        [Inject] private BoardRenderer _boardRenderer { get; set; }
        [Inject] private ChatHubManager _chatHubManager { get; set; }
        [Inject] private AppState _appState { get; set; }


        private bool _initialized = false;
        protected override async Task OnInitializedAsync()
        {
            if (_initialized)
                return;
            else
                _initialized = true;

            _appState.BoardRenderer = _boardRenderer;
            _appState.ChatHubManager = _chatHubManager;

            await _boardRenderer.InitializeAsync();

            _chatHubManager.SetupConnectionAsync();
            _chatHubManager.SetConnectedHandler(ConnectedHanlderAsync);
            await _chatHubManager.StartConnectionAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _chatHubManager.CloseConnectionAsync();
        }


        private async Task ConnectAsync()
        {
            await _chatHubManager.ConnectAsync(_boardId);
        }

        private async Task ConnectedHanlderAsync(string boardId)
        {
            _appState.IconsInstances = new List<DndIconElem>();
            _appState.IconsModels = new List<DndIconElem>();
            _connectedBoardId = boardId;
            StateHasChanged();
            await _appState.InvokeBoardIdChangedAsync(boardId);
        }
    }
}
