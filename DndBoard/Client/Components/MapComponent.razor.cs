using System.Threading.Tasks;
using DndBoard.Client.BaseComponents;
using DndBoard.Client.Helpers;
using DndBoard.Client.Store;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DndBoard.Client.Components
{
    public partial class MapComponent : CanvasBaseComponent
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        private bool _pressed;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0649 // Uninitialized value
        [Inject]
        private CanvasMapRenderer _canvasMapRenderer { get; set; }
        [Inject]
        private AppState _appState { get; set; }
        [Parameter]
        public ChatHubManager ChatHubManager { get; set; }

        protected override void OnInitialized()
        {
            ChatHubManager.SetCoordsReceivedHandler(CoordsReceivedHandler);
            _appState.FilesRefsChanged += Redraw;
        }

        private double _clientX, _clientY;
        private async Task Redraw()
        {
            await _canvasMapRenderer.RedrawImagesByCoords(_clientX, _clientY,
                Canvas, _appState.FilesRefs);
        }

        private async void CoordsReceivedHandler(string message)
        {
            string[] coords = message.Split(":");
            _clientX = double.Parse(coords[0].Trim());
            _clientY = double.Parse(coords[1].Trim());
            await _canvasMapRenderer.RedrawImagesByCoords(_clientX, _clientY,
                Canvas, _appState.FilesRefs);
        }

        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            if (_pressed)
            {
                (double clientX, double clientY) = await GetCanvasCoordinatesAsync(mouseEventArgs);
                await ChatHubManager.SendCoordsAsync($"{clientX} : {clientY}");
            }
        }

        private void OnMouseDown(MouseEventArgs mouseEventArgs) => _pressed = true;
        private void OnMouseUp(MouseEventArgs mouseEventArgs) => _pressed = false;
        private void OnMouseOut(MouseEventArgs mouseEventArgs) => _pressed = false;
    }
}
