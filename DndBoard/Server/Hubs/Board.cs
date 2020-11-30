using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DndBoard.Server.Hubs
{
    public class Board
    {
        private ConcurrentDictionary<string, byte[]> _files =
            new ConcurrentDictionary<string, byte[]>();

        public string BoardId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }


        public byte[] GetFile(string fileId)
        {
            return _files[fileId];
        }

        public void AddFile(byte[] file)
        {
            _files.TryAdd(_files.Count().ToString(), file);
        }

        public IEnumerable<string> GetFilesIds()
        {
            return _files.Keys.ToList();
        }
    }
}
