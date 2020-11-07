using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DndBoard.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Board _board = new Board { X = 33, Y = 99 };


        public async Task SendMessage(string user, string message)
        {
            string[] coords = message.Split(":");
            _board.X = int.Parse(coords[0].Trim());
            _board.Y = int.Parse(coords[1].Trim());
            
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            string con = Context.ConnectionId;
            await Clients.Caller.SendAsync("ReceiveMessage", con, $"{_board.X} : {_board.Y}");
        }
    }
}
