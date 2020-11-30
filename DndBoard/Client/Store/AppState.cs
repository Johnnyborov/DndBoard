using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace DndBoard.Client.Store
{
    public class AppState
    {
        public ElementReference[] FilesRefs;

        public event FilesRefsChangedHandler FilesRefsChanged;
        public event BoardIdChangedHandler BoardIdChanged;

        public async Task InvokeFilesRefsChanged()
            => await FilesRefsChanged?.Invoke();
        public async Task InvokeBoardIdChanged(string boardId)
            => await BoardIdChanged?.Invoke(boardId);
    }
}
