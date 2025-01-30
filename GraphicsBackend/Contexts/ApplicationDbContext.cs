using GraphicsBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public ApplicationDbContext() { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTheme> ProjectThemes { get; set; }
        public DbSet<ProjectGraphic> ProjectGraphics { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
