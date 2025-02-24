
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphicsBackend.Models
{
    public class ProjectTheme
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? JSONData { get; set; }
        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }


    }
}
