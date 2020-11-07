using System;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using DndBoard.Client.BaseComponents;
using DndBoard.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace DndBoard.Client.Pages
{
    public partial class Map : CanvasComponent, IAsyncDisposable
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        private ElementReference _myImage;
        private int _count;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0649 // Uninitialized value

        private HubConnection _hubConnection;
        private CanvasMapRenderer _canvasMapRenderer;

        [Inject]
        private NavigationManager _navigationManager { get; set; }


        public Map()
        {
            _canvasMapRenderer = new CanvasMapRenderer();
        }

        protected override async Task OnInitializedAsync()
        {
            _hubConnection = await SetupSignalRConnectionAsync("/chathub");
            _hubConnection.On<string, string>("ReceiveMessage", ReceiveMessageHandler);
        }

        public async ValueTask DisposeAsync() => await _hubConnection.DisposeAsync();



        private async Task<HubConnection> SetupSignalRConnectionAsync(string hubUri)
        {
            HubConnection connection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri(hubUri))
                .Build();

            await connection.StartAsync();
            return connection;
        }

        private async Task SendAsync(string user, string message)
        {
            await _hubConnection.SendAsync("SendMessage", user, message);
        }

        private void ReceiveMessageHandler(string user, string message)
        {
            string encodedMsg = $"{user}: {message}";
            _jsRuntime.InvokeVoidAsync("alert", $"received: {encodedMsg}")
                .GetAwaiter().GetResult();
        }

        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            await _canvasMapRenderer.RedrawAsync(_myCanvas, _myImage);
            _count++;
        }

        private async Task OnClickAsync(MouseEventArgs mouseEventArgs)
        {
            (double clientX, double clientY) = await GetCanvasCoordinatesAsync(mouseEventArgs);
            await SendAsync("usr1", $"{clientX} : {clientY}");

            await _canvasMapRenderer.RedrawAsync(_myCanvas, _myImage);
            _count++;
        }
    }
}
