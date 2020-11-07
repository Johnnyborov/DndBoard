using System;
using System.Threading.Tasks;
using DndBoard.Client.BaseComponents;
using DndBoard.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DndBoard.Client.Pages
{
    public partial class Map : CanvasBaseComponent, IDisposable
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        private ElementReference _myImage;
        private bool _pressed;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0649 // Uninitialized value
        [Inject]
        private CanvasMapRenderer _canvasMapRenderer { get; set; }
        [Inject]
        private ChatHubManager _chatHubManager { get; set; }


        public void Dispose()
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _chatHubManager.CloseConnectionAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }


        protected override async Task OnInitializedAsync()
        {
            _chatHubManager.SetupConnectionAsync();
            _chatHubManager.SetMessageHandler(ReceiveMessageHandler);
            await _chatHubManager.StartConnectionAsync();
        }


        private async void ReceiveMessageHandler(string user, string message)
        {
            string[] coords = message.Split(":");
            double clientX = double.Parse(coords[0].Trim());
            double clientY = double.Parse(coords[1].Trim());
            await _canvasMapRenderer.RedrawImagesByCoords(clientX, clientY, Canvas, _myImage);
        }

        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            if (_pressed)
            {
                (double clientX, double clientY) = await GetCanvasCoordinatesAsync(mouseEventArgs);
                await _chatHubManager.SendAsync("usr1", $"{clientX} : {clientY}");
            }
        }

        private void OnMouseDown(MouseEventArgs mouseEventArgs) => _pressed = true;
        private void OnMouseUp(MouseEventArgs mouseEventArgs) => _pressed = false;
        private void OnMouseOut(MouseEventArgs mouseEventArgs) => _pressed = false;
    }
}
