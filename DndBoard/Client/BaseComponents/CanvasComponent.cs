using System.Threading.Tasks;
using Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DndBoard.Client.BaseComponents
{
    public class CanvasComponent : ComponentBase
    {
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable CS0649 // Uninitialized value
        protected ElementReference _divCanvas;
        protected BECanvasComponent _myCanvas;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore CS0649 // Uninitialized value
        [Inject]
        protected IJSRuntime _jsRuntime { get; set; }


        protected async Task<(double, double)> GetCanvasCoordinatesAsync(MouseEventArgs mouseEventArgs)
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
    }
}
