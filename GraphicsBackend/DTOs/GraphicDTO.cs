using System.Drawing;
using GraphicsBackend.Models;

namespace GraphicsBackend.DTOs
{
    public class GraphicDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? JSONData { get; set; }

        public ProjectGraphic Create() 
        {
            return new ProjectGraphic
            {
                Id=Guid.NewGuid(),
                ProjectId=ProjectId,
                JSONData=JSONData
            };
        }
    }
}
