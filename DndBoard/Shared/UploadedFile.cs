namespace DndBoard.Shared
{
    public class UploadedFile
    {
        public string BoardId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
