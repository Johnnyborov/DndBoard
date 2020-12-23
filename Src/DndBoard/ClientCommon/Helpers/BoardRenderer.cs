using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndBoard.ClientCommon.Helpers
{
    public class BoardRenderer
    {
        private IJSRuntime _jsRuntime;

        public event RedrawRequestedHandlerAsync RedrawRequestedAsync;


        public BoardRenderer(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }


        public async Task InitializeAsync()
        {
            await _jsRuntime.InvokeVoidAsync(
                "initBoardRendererInstance", DotNetObjectReference.Create(this)
            );
        }

        [JSInvokable]
        public async Task RedrawAsync()
        {
            if (RedrawRequestedAsync is not null)
                await RedrawRequestedAsync.Invoke();
        }
    }
}
