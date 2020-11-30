using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DndBoard.Client.Store;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndBoard.Client.Components
{
    public partial class ImagesComponent : ComponentBase
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        private ElementReference _files;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0649 // Uninitialized value
        private IEnumerable<string> _filesIds;
        private string _boardId;
        private bool _show;

        [Inject]
        private IJSRuntime _jsRuntime { get; set; }
        [Inject]
        private HttpClient _httpClient { get; set; }
        [Inject]
        private AppState _appState { get; set; }


        protected override void OnInitialized()
        {
            _appState.BoardIdChanged += OnBoardIdChanged;
        }

        public async Task OnBoardIdChanged(string boardId)
        {
            _boardId = boardId;
            await ReinitializeFileInput();
            await ReloadFilesIds();
        }

        [JSInvokable]
        public async Task ReloadFilesIds()
        {
            _filesIds = await _httpClient.GetFromJsonAsync<IEnumerable<string>>(
                $"images/getfilesids/{_boardId}"
            );
            StateHasChanged();
        }

        private async Task ReinitializeFileInput()
        {
            await _jsRuntime.InvokeAsync<string>(
                "reinitializeFileInput",
                new object[] { _boardId, _files, DotNetObjectReference.Create(this) }
            );
        }
    }
}
