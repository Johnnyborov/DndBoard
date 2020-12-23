using System.Collections.Generic;
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
            List<ModelFile> files = new();
            foreach (DndIcon icon in board.IconsInstances)
            {
                byte[] modelContent = board.GetFile(icon.ModelId);
                ModelFile file = new() { ModelId = icon.ModelId, ModelContent = modelContent };
                files.Add(file);
            }
            ModelsFiles modelsFiles = new() { BoardId = boardId, Files = files.ToArray() };
            await Clients.Group(boardId).SendAsync(BoardsHubContract.ModelsAdded, modelsFiles);
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

                await Clients.Caller.SendAsync(BoardsHubContract.CoordsChanged, coordsChangeData);
            }
        }


        [HubMethodName(BoardsHubContract.AddModels)]
        public async Task AddModels(ModelsFiles modelsFiles)
        {
            string boardId = modelsFiles.BoardId;
            Board board = _boardsManager.GetBoard(boardId);

            foreach (ModelFile file in modelsFiles.Files)
                file.ModelId = board.AddFile(file.ModelContent);

            await Clients.Group(boardId).SendAsync(BoardsHubContract.ModelsAdded, modelsFiles);
        }

        [HubMethodName(BoardsHubContract.DeleteModel)]
        public async Task DeleteModel(string boardId, string modelId)
        {
            Board board = _boardsManager.GetBoard(boardId);
            board.DeleteFile(modelId);
            await Clients.Group(boardId).SendAsync(BoardsHubContract.ModelDeleted, modelId);
        }


        [HubMethodName(BoardsHubContract.CoordsChanged)]
        public async Task SendCoords(string boardId, CoordsChangeData coordsChangeData)
        {
            Board board = _boardsManager.GetBoard(boardId);

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

            await Clients.Group(boardId).SendAsync(BoardsHubContract.CoordsChanged, coordsChangeData);
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
