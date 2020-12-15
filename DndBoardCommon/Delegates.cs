using System.Threading.Tasks;

namespace DndBoardCommon
{
    public delegate Task FilesRefsChangedHandler();
    public delegate Task BoardIdChangedHandler(string boardId);
}
