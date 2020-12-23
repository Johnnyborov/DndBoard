using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DndBoard.Shared;
using DndBoard.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace DndBoard.ServerCommon.Hubs
{
    public class BoardsHub : Hub
    {
        private readonly BoardsManager _boardsManager;

        public BoardsHub(BoardsManager boardsManager)
        {
            _boardsManager = boardsManager;
        }


        [HubMethodName(BoardsHubContract.RequestAllModels)]
        public async Task RequestAllModels(string boardId)
        {
            Board board = _boardsManager.GetBoard(boardId);
            foreach (DndIcon icon in board.IconsInstances)
            {
                byte[] model = board.GetFile(icon.ModelId);
                UploadedFiles uploadedFiles = new UploadedFiles();
                uploadedFiles.BoardId = boardId;
                uploadedFiles.Files = new UploadedFile[]
                {
                    new UploadedFile { FileName = icon.ModelId, FileContent = model }
                };
                await Clients.Group(boardId).SendAsync(BoardsHubContract.ModelsAdded, uploadedFiles);
            }
        }

        [HubMethodName(BoardsHubContract.RequestAllCoords)]
        public async Task RequestAllCoords(string boardId)
        {
            Board board = _boardsManager.GetBoard(boardId);
            foreach (DndIcon icon in board.IconsInstances)
            {
                CoordsChangeData coordsChangeData = new CoordsChangeData
                {
                    InstanceId = icon.InstanceId,
                    Coords = icon.Coords,
                    ModelId = icon.ModelId,
                };

                string coordsChangeDataJson = JsonSerializer.Serialize(coordsChangeData);
                await Clients.Caller.SendAsync(BoardsHubContract.CoordsChanged, coordsChangeDataJson);
            }
        }


        [HubMethodName(BoardsHubContract.AddModels)]
        public async Task AddModels(UploadedFiles uploadedFiles)
        {
            string boardId = uploadedFiles.BoardId;
            Board board = _boardsManager.GetBoard(boardId);

            foreach (UploadedFile file in uploadedFiles.Files)
                file.FileName = board.AddFile(file.FileContent);

            await Clients.Group(boardId).SendAsync(BoardsHubContract.ModelsAdded, uploadedFiles);
        }

        [HubMethodName(BoardsHubContract.DeleteModel)]
        public async Task DeleteModel(string boardId, string fileId)
        {
            Board board = _boardsManager.GetBoard(boardId);
            board.DeleteFile(fileId);
            await Clients.Group(boardId).SendAsync(BoardsHubContract.ModelDeleted, boardId);
        }


        [HubMethodName(BoardsHubContract.CoordsChanged)]
        public async Task SendCoords(string boardId, string coordsChangeDataJson)
        {
            Board board = _boardsManager.GetBoard(boardId);
            CoordsChangeData coordsChangeData = JsonSerializer
                .Deserialize<CoordsChangeData>(coordsChangeDataJson);

            if (!board.IconsInstances.Exists(icon => icon.InstanceId == coordsChangeData.InstanceId))
                board.IconsInstances.Add(new DndIcon
                {
                    InstanceId = coordsChangeData.InstanceId,
                    Coords = coordsChangeData.Coords,
                    ModelId = coordsChangeData.ModelId,
                });
            else
                board.IconsInstances.Find(icon => icon.InstanceId == coordsChangeData.InstanceId)
                    .Coords = coordsChangeData.Coords;

            await Clients.Group(boardId).SendAsync(BoardsHubContract.CoordsChanged, coordsChangeDataJson);
        }

        [HubMethodName(BoardsHubContract.IconInstanceRemoved)]
        public async Task SendIconInstanceRemoved(string boardId, string instanceId)
        {
            Board board = _boardsManager.GetBoard(boardId);
            board.IconsInstances.RemoveAll(icon => icon.InstanceId == instanceId);
            await Clients.Group(boardId).SendAsync(BoardsHubContract.IconInstanceRemoved, instanceId);
        }


        [HubMethodName(BoardsHubContract.Connect)]
        public async Task Connect(string boardId)
        {
            if (!_boardsManager.BoardExists(boardId))
                _boardsManager.AddBoard(new Board
                {
                    BoardId = boardId,
                    IconsInstances = new List<DndIcon>()
                });

            await Groups.AddToGroupAsync(Context.ConnectionId, boardId);
            await Clients.Caller.SendAsync(BoardsHubContract.Connected, boardId);
        }
    }
}
