﻿using System.Threading.Tasks;
using DndBoardServer.Hubs;
using DndBoard.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DndBoardServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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



        [HttpPost("{boardId}/{fileId}")]
        public async Task<IActionResult> DeleteFile(string boardId, string fileId)
        {
            Board board = _boardManager.GetBoard(boardId);
            board.DeleteFile(fileId);

            await _boardsHubContext.Clients.Group(boardId)
                .SendAsync(BoardsHubContract.NotifyFilesUpdate, boardId);

            return Ok();
        }

        [HttpPost]
        public async Task PostFiles(UploadedFiles uploadedFiles)
        {
            string boardId = uploadedFiles.BoardId;
            Board board = _boardManager.GetBoard(boardId);

            foreach (UploadedFile file in uploadedFiles.Files)
                board.AddFile(file.FileContent);

            await _boardsHubContext.Clients.Group(boardId)
                .SendAsync(BoardsHubContract.NotifyFilesUpdate, boardId);
        }
    }
}
