using System.Collections.Generic;
using System.Threading.Tasks;
using DndBoardCommon.Helpers;
using DndBoardCommon.Models;

namespace DndBoardCommon.Store
{
    public class AppState
    {
        public ChatHubManager ChatHubManager;
        public List<MapImage> MapImages;
        public List<MapImage> ModelImages;

        public event FilesRefsChangedHandler FilesRefsChanged;
        public event BoardIdChangedHandler BoardIdChanged;

        public async Task InvokeFilesRefsChanged()
        { // null conditional operator doesn't work. GG.
            if (FilesRefsChanged is not null)
                await FilesRefsChanged.Invoke();
        }
        public async Task InvokeBoardIdChanged(string boardId)
            => await BoardIdChanged?.Invoke(boardId);
    }
}
