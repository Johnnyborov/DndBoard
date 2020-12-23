using System.Threading.Tasks;

namespace DndBoard.ClientCommon
{
    public delegate Task RedrawRequestedHandlerAsync();
    public delegate Task FilesRefsChangedHandlerAsync();
    public delegate Task BoardIdChangedHandlerAsync(string boardId);
}
