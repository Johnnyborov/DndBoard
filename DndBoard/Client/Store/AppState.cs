using System.Threading.Tasks;

namespace DndBoard.Client.Store
{
    public class AppState
    {
        public event BoardIdChangedHandler BoardIdChanged;

        public async Task InvokeBoardIdChanged(string boardId)
        {
            await BoardIdChanged?.Invoke(boardId);
        }
    }
}
