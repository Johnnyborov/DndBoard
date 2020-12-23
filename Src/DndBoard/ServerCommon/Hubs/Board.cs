using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DndBoard.Shared.Models;

namespace DndBoard.ServerCommon.Hubs
{
    public class Board
    {
        private readonly ConcurrentDictionary<string, byte[]> _files =
            new ConcurrentDictionary<string, byte[]>();

        public string BoardId { get; set; }
        public List<DndIcon> IconsInstances { get; set; }


        public byte[] GetFile(string fileId)
        {
            return _files[fileId];
        }

        public void DeleteFile(string fileId)
        {
            _files.Remove(fileId, out _);
        }

        public string AddFile(byte[] file)
        {
            string id = Guid.NewGuid().ToString();
            _files.TryAdd(id, file);
            return id;
        }

        public IEnumerable<string> GetFilesIds()
        {
            return _files.Keys.ToList();
        }
    }
}
