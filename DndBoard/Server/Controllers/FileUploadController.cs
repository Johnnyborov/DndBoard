using System.Threading.Tasks;
using DndBoard.Server.Hubs;
using DndBoard.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DndBoard.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileUploadController : ControllerBase
    {
        private readonly BoardsManager _boardManager;
        private readonly IHubContext<BoardsHub> _boardsHubContext;

        public FileUploadController(
            BoardsManager boardManager,
            IHubContext<BoardsHub> boardsHubContext)
        {
            _boardManager = boardManager;
            _boardsHubContext = boardsHubContext;
        }


        [HttpPost]
        public async Task PostFiles(UploadedFile uploadedFile)
        {
            string boardId = uploadedFile.BoardId;
            {
                Board board = _boardManager.GetBoard(boardId);
                board.AddFile(uploadedFile.FileContent);
            }
            await _boardsHubContext.Clients.Group(boardId)
                .SendAsync(BoardsHubContract.NotifyFilesUpdate, boardId);
        }
    }
}
