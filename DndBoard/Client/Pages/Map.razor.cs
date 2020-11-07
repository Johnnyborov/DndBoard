using System;
using System.Threading.Tasks;
using DndBoard.Client.BaseComponents;
using DndBoard.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DndBoard.Client.Pages
{
    public partial class Map : CanvasBaseComponent, IDisposable
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        private ElementReference _myImage;
        private int _count;
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
            await _chatHubManager.StartConnectionAsync();
            _chatHubManager.SetMessageHandler(ReceiveMessageHandler);

            await _canvasMapRenderer.RedrawTestImageAsync(Canvas, _myImage);
            _count++;
        }


        private void ReceiveMessageHandler(string user, string message)
        {
            string encodedMsg = $"{user}: {message}";
            JsRuntime.InvokeVoidAsync("alert", $"received: {encodedMsg}")
                .GetAwaiter().GetResult();
        }

        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            await _canvasMapRenderer.RedrawTestImageAsync(Canvas, _myImage);
            _count++;
        }

        private async Task OnClickAsync(MouseEventArgs mouseEventArgs)
        {
            (double clientX, double clientY) = await GetCanvasCoordinatesAsync(mouseEventArgs);
            await _chatHubManager.SendAsync("usr1", $"{clientX} : {clientY}");

            await _canvasMapRenderer.RedrawTestImageAsync(Canvas, _myImage);
            _count++;
        }
    }
}
