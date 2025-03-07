using GraphicsBackend.Models;

namespace GraphicsBackend.DTOs
{
    public class ThemeDTO
    {
        public Guid? Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? JSONData { get; set; }

        public ProjectTheme Create() 
        {
            return new ProjectTheme
            {
                Id = Guid.NewGuid(),
                ProjectId = ProjectId,
                JSONData = JSONData,
            };
        }
    }
}
