using System.Collections.Generic;
using System.Threading.Tasks;
using DndBoard.Shared;

namespace DndBoard.ClientCommon
{
    public interface IFilesClient
    {
        public Task PostFilesAsJsonAsync(UploadedFiles value);
        public Task DeleteFilesAsync(string boardId, string fileId);
        Task<List<string>> GetFilesListAsJsonAsync(string boardId);
    }
}
