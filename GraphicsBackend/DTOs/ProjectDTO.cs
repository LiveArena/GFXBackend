using GraphicsBackend.Models;
using Pipelines.Sockets.Unofficial.Arenas;

namespace GraphicsBackend.DTOs
{
    public class ProjectDTO
    {
        public Guid? Id { get; set; }
        public int? CustomerId { get; set; }
        public string? JSONData { get; set; }

        public Project Create()
        {
            return new Project
            {
                Id = Guid.NewGuid(),
                CustomerId=CustomerId,
                JSONData=JSONData
            };
            
        }
    }
}
