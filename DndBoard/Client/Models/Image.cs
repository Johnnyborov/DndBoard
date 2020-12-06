using Microsoft.AspNetCore.Components;

namespace DndBoard.Client.Models
{
    public class Image
    {
        public string FileId { get; init; }
        public ElementReference Ref { get; set; }
    }
}
