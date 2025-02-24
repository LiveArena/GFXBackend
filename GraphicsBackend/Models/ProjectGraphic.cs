using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphicsBackend.Models
{
    public class ProjectGraphic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? JSONData { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        public bool Hide { get; set; }
    }
}
