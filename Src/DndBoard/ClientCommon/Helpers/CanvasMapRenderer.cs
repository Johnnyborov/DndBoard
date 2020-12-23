using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using DndBoard.ClientCommon.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DndBoard.ClientCommon.Helpers
{
    public class CanvasMapRenderer
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);


        public async Task RedrawIconsByCoordsJS(
            string divCanvasId,
            IJSRuntime jsRuntime,
            List<DndIconElem> icons)
        {
            await _semaphore.WaitAsync();
            try
            {
                List<object> imgList = new();
                foreach (DndIconElem icon in icons)
                    imgList.Add(new { icon.Ref, icon.Coords.X, icon.Coords.Y });

                await jsRuntime.InvokeAsync<object>(
                    "redrawAllImages",
                    new object[] { divCanvasId, imgList }
                );
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
