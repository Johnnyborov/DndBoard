﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using DndBoard.Shared;
using Microsoft.AspNetCore.Components;

namespace DndBoard.Client.Helpers
{
    public class CanvasMapRenderer
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);
        public static async Task RedrawImagesByCoords(
            List<Coords> coords,
            BECanvasComponent canvas,
            ElementReference[] images)
        {
            await _semaphore.WaitAsync();
            try
            {
                Canvas2DContext context = await canvas.CreateCanvas2DAsync();
                await context.ClearRectAsync(0, 0, canvas.Width, canvas.Height);
                await context.SetFillStyleAsync("Red");
                await context.FillRectAsync(0, 0, canvas.Width, canvas.Height);

                int i = 0;
                foreach (var img in images)
                {
                    if (i >= coords.Count)
                        return;

                    await context.DrawImageAsync(img, coords[i].X, coords[i].Y);
                    i++;
                }
            }
            catch(Exception ex)
            {
                var x = ex;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public static async Task RedrawTestImageAsync(BECanvasComponent canvas, ElementReference image)
        {
            Canvas2DContext context = await canvas.CreateCanvas2DAsync();


            await context.ClearRectAsync(0, 0, canvas.Width, canvas.Height);
            await context.SetFillStyleAsync("Red");
            await context.FillRectAsync(0, 0, canvas.Width, canvas.Height);

            await context.SetFillStyleAsync("Green");
            await context.FillRectAsync(10, 10, canvas.Width - 20, canvas.Height - 20);


            await context.SaveAsync();
            for (int i = 0; i < 6; i++)
            {
                await context.SetFillStyleAsync($"#{i}F0000");
                await context.BeginPathAsync();
                await context.MoveToAsync(350, 350);
                await context.ArcAsync(350, 350, 200,
                    (Math.PI / 180) * 60 * i,
                    (Math.PI / 180) * 60 * (i + 1), false);
                await context.ClosePathAsync();
                await context.FillAsync();
            }
            await context.RestoreAsync();

            await context.DrawImageAsync(image, 22, 33);
        }
    }
}
