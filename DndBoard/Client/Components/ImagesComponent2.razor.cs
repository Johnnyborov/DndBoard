using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DndBoard.Client.BaseComponents;
using DndBoard.Client.Helpers;
using DndBoard.Client.Models;
using DndBoard.Client.Store;
using DndBoard.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace DndBoard.Client.Components
{
    public partial class ImagesComponent2 : CanvasBaseComponent
    {
        private List<Image> _images = new();
        private string _boardId;
        private readonly List<Coords> _coords = new();

        [Inject]
        private HttpClient _httpClient { get; set; }
        [Inject]
        private AppState _appState { get; set; }


        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            int maxAllowedFiles = 777;

            List<UploadedFile> files = new();
            foreach (IBrowserFile imageFile in e.GetMultipleFiles(maxAllowedFiles))
            {
                UploadedFile uploadedFile = await ConvertToUploadedFile(imageFile);
                files.Add(uploadedFile);
            }

            await SendFilesToServer(files);
        }

        private static async Task<UploadedFile> ConvertToUploadedFile(IBrowserFile imageFile)
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

        private async Task SendFilesToServer(List<UploadedFile> files)
        {
            UploadedFiles uploadedFiles = new();
            uploadedFiles.BoardId = _boardId;
            uploadedFiles.Files = files.ToArray();

            await _httpClient.PostAsJsonAsync(
                "/fileupload/PostFiles", uploadedFiles
            );
        }

        protected override void OnInitialized()
        {
            _appState.BoardIdChanged += OnBoardIdChanged;
            _appState.ChatHubManager.SetNofifyFilesUpdateHandler(OnFilesUpdated);
            _appState.FilesRefsChanged += Redraw;
        }

        private void OnFilesUpdated(string boardId)
        {
            _ = RefreshFilesAsync();
            async Task RefreshFilesAsync()
            {
                await ReloadFiles();
                _appState.FilesRefs = _images.Select(i => i.Ref).ToArray();
                await _appState.InvokeFilesRefsChanged();
            }
        }

        private async Task Redraw()
        {
            _coords.Clear();
            for (int i = 0; i < _appState.FilesRefs.Length; i++)
                _coords.Add(new Coords { X = 50, Y = 50 + i * 100 });

            await CanvasMapRenderer.RedrawImagesByCoords(_coords,
                Canvas, _appState.FilesRefs);
        }

        private async Task OnBoardIdChanged(string boardId)
        {
            _boardId = boardId;
            OnFilesUpdated(_boardId);
        }

        private async Task ReloadFiles()
        {
            List<string> fileIds = await _httpClient.GetFromJsonAsync<List<string>>(
                $"images/getfilesids/{_boardId}"
            );
            _images = new(); // Otherwise existing refs don't get updated.
            StateHasChanged();
            _images = fileIds.Select(id => new Image { FileId = id }).ToList();
            StateHasChanged();
            await Task.Delay(300); // Wait for images to get downloaded. Use loaded event?
        }
    }
}


//var img = document.querySelector('img')

//function loaded() {
//  alert('loaded');
//}

//if (img.complete) {
//  loaded();
//} else {
//  img.addEventListener('load', loaded);
//  img.addEventListener('error', function() {
//      alert('error')
//  });
//}
