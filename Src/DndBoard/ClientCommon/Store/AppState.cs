using System.Collections.Generic;
using System.Threading.Tasks;
using DndBoard.ClientCommon.Helpers;
using DndBoard.ClientCommon.Models;

namespace DndBoard.ClientCommon.Store
{
    public class AppState
    {
        public BoardRenderer BoardRenderer { get; set; }
        public ChatHubManager ChatHubManager { get; set; }
        public List<DndIconElem> IconsInstances { get; set; }
        public List<DndIconElem> IconsModels { get; set; }

        public event FilesRefsChangedHandler FilesRefsChanged;
        public event BoardIdChangedHandler BoardIdChanged;


        public async Task InvokeFilesRefsChanged()
        {
            if (FilesRefsChanged is not null)
                await FilesRefsChanged.Invoke();
        }

        public async Task InvokeBoardIdChanged(string boardId)
        {
            if (BoardIdChanged is not null)
                await BoardIdChanged.Invoke(boardId);
        }
    }
}
