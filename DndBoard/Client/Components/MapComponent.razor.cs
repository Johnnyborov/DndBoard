using System;
using System.Text.Json;
using System.Threading.Tasks;
using DndBoard.Client.BaseComponents;
using DndBoard.Client.Helpers;
using DndBoard.Client.Models;
using DndBoard.Client.Store;
using DndBoard.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace DndBoard.Client.Components
{
    public partial class MapComponent : CanvasBaseComponent
    {
        private bool _pressed;
        [Inject]
        private AppState _appState { get; set; }


        protected override void OnInitialized()
        {
            _appState.ChatHubManager.SetCoordsReceivedHandler(CoordsReceivedHandler);
            _appState.FilesRefsChanged += OnFileRefsChanged;
            _appState.BoardIdChanged += boardId => Redraw();
        }

        private async Task OnFileRefsChanged()
        {

            await Redraw();
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

            await Redraw();
        }

        private async Task Redraw()
        {
            await CanvasMapRenderer.RedrawImagesByCoords(Canvas, _appState.MapImages);
        }

        private async Task OnMouseMoveAsync(MouseEventArgs mouseEventArgs)
        {
            if (_pressed)
            {
                Coords coords = await GetCanvasCoordinatesAsync(mouseEventArgs);
                MapImage clickedImage = GetClickedImage(coords);
                clickedImage.Coords = coords;

                CoordsChangeData coordsChangeData = new CoordsChangeData
                {
                    ImageId = clickedImage.Id,
                    Coords = coords,
                    ModelId = clickedImage.ModelId,
                };
                string coordsChangeDataJson = JsonSerializer.Serialize(coordsChangeData);
                await _appState.ChatHubManager.SendCoordsAsync(coordsChangeDataJson);
            }
        }

        private MapImage GetClickedImage(Coords coords)
        {
            return _appState.MapImages[0];
        }

        private void OnMouseDown(MouseEventArgs mouseEventArgs) => _pressed = true;
        private void OnMouseUp(MouseEventArgs mouseEventArgs) => _pressed = false;
        private void OnMouseOut(MouseEventArgs mouseEventArgs) => _pressed = false;
    }
}
