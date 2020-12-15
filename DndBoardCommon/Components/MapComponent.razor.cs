using System.Text.Json;
using System.Threading.Tasks;
using DndBoardCommon.BaseComponents;
using DndBoardCommon.Helpers;
using DndBoardCommon.Models;
using DndBoardCommon.Store;
using DndBoard.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DndBoardCommon.Components
{
    public partial class MapComponent : CanvasBaseComponent
    {
        private MapImage _clickedImage;
        [Inject]
        private AppState _appState { get; set; }
        [Inject]
        private IJSRuntime _jsRuntime { get; set; }

        private bool _initialized = false;
        protected override async Task OnInitializedAsync()
        {
            if (_initialized)
                return;
            else
                _initialized = true;

            _appState.ChatHubManager.SetCoordsReceivedHandler(CoordsReceivedHandler);
            _appState.ChatHubManager.SetImageRemovedHandler(ImageRemovedHandler);
            _appState.FilesRefsChanged += OnFileRefsChanged;
            _appState.BoardIdChanged += boardId => QueueRedraw();

            await _jsRuntime.InvokeAsync<object>(
                "initMapComponent", DotNetObjectReference.Create(this)
            );
        }

        private async Task OnFileRefsChanged()
        {
            await QueueRedraw();
        }

        private async void ImageRemovedHandler(string imageId)
        {
            _appState.MapImages.RemoveAll(img => img.Id == imageId);
            await QueueRedraw();
        }

        private async void CoordsReceivedHandler(string coordsChangeDataJson)
        {
            CoordsChangeData coordsChangeData = JsonSerializer
                .Deserialize<CoordsChangeData>(coordsChangeDataJson);

            if (!_appState.MapImages.Exists(img => img.Id == coordsChangeData.ImageId)
                && _appState.ModelImages.Exists(x => x.Id == coordsChangeData.ModelId))
            {
                _appState.MapImages.Add(new MapImage
                {
                    Id = coordsChangeData.ImageId,
                    Coords = coordsChangeData.Coords,
                    ModelId = coordsChangeData.ModelId,
                    Ref = _appState.ModelImages.Find(x => x.Id == coordsChangeData.ModelId).Ref,
                });
            }

            if (_appState.MapImages.Exists(img => img.Id == coordsChangeData.ImageId))
            {
                _appState.MapImages.Find(img => img.Id == coordsChangeData.ImageId)
                    .Coords = coordsChangeData.Coords;
            }

            await QueueRedraw();
        }

        private async Task QueueRedraw()
        {
            await _jsRuntime.InvokeAsync<object>("queueRedraw");
        }

        [JSInvokable]
        public async Task Redraw()
        {
            if (_appState.MapImages is null)
                return;

            await CanvasMapRenderer.RedrawImagesByCoords(Canvas, _appState.MapImages);
        }

        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            MapImage mapImage = _clickedImage;
            if (mapImage is not null)
            {
                Coords coords = await GetCanvasCoordinatesAsync(mouseEventArgs);

                mapImage.Coords = coords;

                CoordsChangeData coordsChangeData = new CoordsChangeData
                {
                    ImageId = mapImage.Id,
                    Coords = coords,
                    ModelId = mapImage.ModelId,
                };
                string coordsChangeDataJson = JsonSerializer.Serialize(coordsChangeData);
                await _appState.ChatHubManager.SendCoordsAsync(coordsChangeDataJson);
            }
        }

        private async Task<MapImage> GetClickedImage(MouseEventArgs mouseEventArgs)
        {
            Coords coords = await GetCanvasCoordinatesAsync(mouseEventArgs);

            foreach (MapImage img in _appState.MapImages)
            {
                if (coords.X >= img.Coords.X && coords.X <= img.Coords.X + 100
                    && coords.Y >= img.Coords.Y && coords.Y <= img.Coords.Y + 100)
                {
                    return img;
                }
            }

            return null;
        }

        private async Task OnMouseDown(MouseEventArgs mouseEventArgs) =>
            _clickedImage = await GetClickedImage(mouseEventArgs);
        private void OnMouseUp(MouseEventArgs mouseEventArgs) =>
            _clickedImage = null;
        private void OnMouseOut(MouseEventArgs mouseEventArgs) =>
            _clickedImage = null;

        private async Task OnRightClick(MouseEventArgs mouseEventArgs)
        {
            MapImage clickedImage = await GetClickedImage(mouseEventArgs);
            if (clickedImage is not null)
                await _appState.ChatHubManager.SendImageRemoved(clickedImage.Id);
        }
    }
}
