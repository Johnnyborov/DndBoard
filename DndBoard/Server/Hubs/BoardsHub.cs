using System.Threading.Tasks;
using DndBoard.Shared;
using Microsoft.AspNetCore.SignalR;

namespace DndBoard.Server.Hubs
{
    public class BoardsHub : Hub
    {
        private readonly BoardsManager _boardsManager;

        public BoardsHub(BoardsManager boardsManager)
        {
            _boardsManager = boardsManager;
        }


        [HubMethodName(BoardsHubContract.CoordsChanged)]
        public async Task SendCoords(string boardId, string coords)
        {
            Board board = _boardsManager.GetBoard(boardId);
            string[] coordsArr = coords.Split(":");
            board.X = int.Parse(coordsArr[0].Trim());
            board.Y = int.Parse(coordsArr[1].Trim());

            await Clients.All.SendAsync(BoardsHubContract.CoordsChanged, coords);
        }

        [HubMethodName(BoardsHubContract.Connect)]
        public async Task Connect(string boardId)
        {
            if (!_boardsManager.BoardExists(boardId))
                _boardsManager.AddBoard(new Board { BoardId = boardId, X = 33, Y = 111 });

            Board board = _boardsManager.GetBoard(boardId);
            await Clients.Caller.SendAsync(BoardsHubContract.Connected, boardId);
            await Clients.Caller.SendAsync(BoardsHubContract.CoordsChanged, $"{board.X} : {board.Y}");
        }
    }
}
