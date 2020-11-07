using System;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DndBoard.Client.Pages
{
    public partial class MapComponent : IAsyncDisposable
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        private ElementReference _myImage;
        private ElementReference _divCanvas;
        private BECanvasComponent _myCanvas;
        private int _count;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0649 // Uninitialized value

        private Canvas2DContext _canvasContext;
        private HubConnection _hubConnection;

        [Inject]
        private NavigationManager _navigationManager { get; set; }
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }


        protected override async Task OnInitializedAsync()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                string encodedMsg = $"{user}: {message}";
                _jsRuntime.InvokeVoidAsync("alert", $"received: {encodedMsg}")
                    .GetAwaiter().GetResult();
            });

            await _hubConnection.StartAsync();
        }

        private Task SendAsync(string user, string message) =>
            _hubConnection.SendAsync("SendMessage", user, message);
        public async ValueTask DisposeAsync() => await _hubConnection.DisposeAsync();


        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            await RedrawAsync(mouseEventArgs);
        }

        private async Task OnClickAsync(MouseEventArgs mouseEventArgs)
        {
            (double clientX, double clientY) = await GetCanvasCoordinatesAsync(mouseEventArgs);
            await SendAsync("usr1", $"{clientX} : {clientY}");
            await RedrawAsync(mouseEventArgs);
        }

        private async Task<(double, double)> GetCanvasCoordinatesAsync(MouseEventArgs mouseEventArgs)
        {
            string data = await _jsRuntime.InvokeAsync<string>(
                "getElementOffsets",
                new object[] { _divCanvas }
            );

            JObject offsets = (JObject)JsonConvert.DeserializeObject(data);
            double mouseX = mouseEventArgs.ClientX - offsets.Value<double>("offsetLeft");
            double mouseY = mouseEventArgs.ClientY - offsets.Value<double>("offsetTop");
            return (mouseX, mouseY);
        }

        private async Task RedrawAsync(MouseEventArgs mouseEventArgs)
        {
            _count++;
            _canvasContext = await _myCanvas.CreateCanvas2DAsync();


            await _canvasContext.ClearRectAsync(0, 0, _myCanvas.Width, _myCanvas.Height);
            await _canvasContext.SetFillStyleAsync("Red");
            await _canvasContext.FillRectAsync(0, 0, _myCanvas.Width, _myCanvas.Height);

            await _canvasContext.SetFillStyleAsync("Green");
            await _canvasContext.FillRectAsync(10, 10, _myCanvas.Width - 20, _myCanvas.Height - 20);


            await _canvasContext.SaveAsync();
            for (int i = 0; i < 6; i++)
            {
                await _canvasContext.SetFillStyleAsync($"#{i}F0000");
                await _canvasContext.BeginPathAsync();
                await _canvasContext.MoveToAsync(350, 350);
                await _canvasContext.ArcAsync(350, 350, 200,
                    (Math.PI / 180) * 60 * i,
                    (Math.PI / 180) * 60 * (i + 1), false);
                await _canvasContext.ClosePathAsync();
                await _canvasContext.FillAsync();
            }
            await _canvasContext.RestoreAsync();

            await _canvasContext.DrawImageAsync(_myImage, 22, 33);
        }
    }
}
