using System.ComponentModel.DataAnnotations;

namespace GraphicsBackend.Models
{
    public class Customer
    {
        [Key]
        public required int Id { get; set; }
        public string? Name { get; set; }
        public string? Tenant { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
}
