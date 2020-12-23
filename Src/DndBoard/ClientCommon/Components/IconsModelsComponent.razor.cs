using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DndBoard.ClientCommon.BaseComponents;
using DndBoard.ClientCommon.Helpers;
using DndBoard.ClientCommon.Models;
using DndBoard.ClientCommon.Store;
using DndBoard.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace DndBoard.ClientCommon.Components
{
    public partial class IconsModelsComponent : CanvasBaseComponent
    {
        private string _boardId;
        [Inject] private CanvasMapRenderer _canvasMapRenderer { get; set; }
        [Inject] private AppState _appState { get; set; }
        [Inject] private IJSRuntime _jsRuntime { get; set; }


        private async Task OnInputFileChangeAsync(InputFileChangeEventArgs e)
        {
            int maxAllowedFiles = 777;

            List<UploadedFile> files = new();
            foreach (IBrowserFile imageFile in e.GetMultipleFiles(maxAllowedFiles))
            {
                UploadedFile uploadedFile = await ConvertToUploadedFileAsync(imageFile);
                files.Add(uploadedFile);
            }

            await SendFilesToServerAsync(files);
        }

        private static async Task<UploadedFile> ConvertToUploadedFileAsync(IBrowserFile imageFile)
        {
            string format = "image/png";

            IBrowserFile resizedImageFile = await imageFile
                .RequestImageFileAsync(format, 100, 100);

            Stream stream = resizedImageFile.OpenReadStream();
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            stream.Close();

            UploadedFile uploadedFile = new();
            uploadedFile.FileName = resizedImageFile.Name;
            uploadedFile.FileContent = ms.ToArray();
            ms.Close();

            return uploadedFile;
        }

        private async Task SendFilesToServerAsync(List<UploadedFile> files)
        {
            UploadedFiles uploadedFiles = new();
            uploadedFiles.BoardId = _boardId;
            uploadedFiles.Files = files.ToArray();

            await _appState.ChatHubManager.AddModelsAsync(uploadedFiles);
        }

        private async Task OnRightClickAsync(MouseEventArgs mouseEventArgs)
        {
            Coords coords = await GetCanvasCoordinatesAsync(mouseEventArgs);
            DndIconElem clickedIcon = GetClickedIcon(coords);
            if (clickedIcon is null)
                return;

            await _appState.ChatHubManager.DeleteModelAsync(clickedIcon.ModelId);
        }

        private async Task OnClickAsync(MouseEventArgs mouseEventArgs)
        {
            Coords coords = await GetCanvasCoordinatesAsync(mouseEventArgs);
            DndIconElem clickedIcon = GetClickedIcon(coords);
            if (clickedIcon is null)
                return;
 
            CoordsChangeData coordsChangeData = new CoordsChangeData
            {
                InstanceId = Guid.NewGuid().ToString(),
                Coords = new Coords { X = 10, Y = 10 },
                ModelId = clickedIcon.ModelId,
            };
            string coordsChangeDataJson = JsonSerializer.Serialize(coordsChangeData);
            await _appState.ChatHubManager.SendCoordsAsync(coordsChangeDataJson);
        }

        private DndIconElem GetClickedIcon(Coords coords)
        {
            foreach (DndIconElem icon in _appState.IconsModels)
            {
                if (coords.X >= icon.Coords.X && coords.X <= icon.Coords.X + 100
                    && coords.Y >= icon.Coords.Y && coords.Y <= icon.Coords.Y + 100)
                {
                    return icon;
                }
            }

            return null;
        }

        private bool _initialized = false;
        protected override void OnInitialized()
        {
            if (_initialized)
                return;
            else
                _initialized = true;

            _appState.BoardIdChangedAsync += OnBoardIdChangedAsync;
            _appState.BoardRenderer.RedrawRequestedAsync += OnRedrawAsync;
            _appState.ChatHubManager.ModelsAddedAsync += OnModelAddedAsync;
            _appState.ChatHubManager.ModelDeleted += OnModelDeletedAsync;
        }

        private async Task OnModelAddedAsync(UploadedFiles uploadedFiles)
        {
            List<DndIconElem> newModels = await CreateNewIconsModelsAsync(uploadedFiles);

            newModels.RemoveAll(x => _appState.IconsModels.Any(y => y.ModelId == x.ModelId));
            _appState.IconsModels.AddRange(newModels.Except(_appState.IconsModels));

            for (int i = 0; i < _appState.IconsModels.Count; i++)
                _appState.IconsModels[i].Coords = new Coords { X = 50, Y = 50 + i * 110 };

            StateHasChanged();
            await _appState.InvokeAllModelsLoadedAsync();
        }

        private async Task<List<DndIconElem>> CreateNewIconsModelsAsync(UploadedFiles uploadedFiles)
        {
            List<DndIconElem> newModels;

            if (_jsRuntime is IJSUnmarshalledRuntime jsUnmarshalledRuntime)
            {
                newModels = uploadedFiles.Files
                    .Select(file =>
                    {
                        string url = jsUnmarshalledRuntime.InvokeUnmarshalled<byte[], string>(
                            "createFileURLUnmarshalled", file.FileContent
                        );

                        return new DndIconElem { ModelId = file.FileName, Url = url };
                    }).ToList();
            }
            else
            {
                List<Task<DndIconElem>> newModelsTasks = uploadedFiles.Files
                    .Select(async file =>
                    {
                        string url = await _jsRuntime.InvokeAsync<string>(
                            "createFileURL", file.FileContent
                        );

                        return new DndIconElem { ModelId = file.FileName, Url = url };
                    }).ToList();

                await Task.WhenAll(newModelsTasks);
                newModels = newModelsTasks.Select(task => task.Result).ToList();
            }

            return newModels;
        }

        private void OnModelDeletedAsync(string modelId)
        {
            _appState.IconsModels.RemoveAll(icon => icon.ModelId == modelId);
        }

        private async Task OnRedrawAsync()
        {
            if (_appState.IconsModels is null)
                return;

            await _canvasMapRenderer.RedrawIconsByCoordsAsync("IconsModelsDivCanvas", _jsRuntime, _appState.IconsModels);
        }

        private async Task OnBoardIdChangedAsync(string boardId)
        {
            _boardId = boardId;
            StateHasChanged();
            await _appState.ChatHubManager.RequestAllModelsAsync();
        }
    }
}
