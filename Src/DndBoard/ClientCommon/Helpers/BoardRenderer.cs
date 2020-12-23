using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndBoard.ClientCommon.Helpers
{
    public class BoardRenderer
    {
        private IJSRuntime _jsRuntime;

        public event RedrawRequestedHandler RedrawRequested;


        public BoardRenderer(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }


        public async Task Initialize()
        {
            await _jsRuntime.InvokeVoidAsync(
                "initBoardRendererInstance", DotNetObjectReference.Create(this)
            );
        }

        [JSInvokable]
        public async Task Redraw()
        {
            if (RedrawRequested is not null)
                await RedrawRequested.Invoke();
        }
    }
}
