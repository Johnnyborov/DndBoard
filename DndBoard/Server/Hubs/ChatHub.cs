using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DndBoard.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, Board> _boards =
            new ConcurrentDictionary<string, Board>();


        public async Task SendCoords(string boardId, string message)
        {
            Board board = _boards[boardId];
            string[] coords = message.Split(":");
            board.X = int.Parse(coords[0].Trim());
            board.Y = int.Parse(coords[1].Trim());
            
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task Connect(string boardId)
        {
            if (!_boards.ContainsKey(boardId))
                _boards.TryAdd(boardId, new Board { X = 33, Y = 111 });

            Board board = _boards[boardId];
            await Clients.Caller.SendAsync("ReceiveMessage", $"{board.X} : {board.Y}");
        }
    }
}
