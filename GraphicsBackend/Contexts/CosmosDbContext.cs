using GraphicsBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Contexts
{
    public class CosmosDbContext : DbContext
    {
        public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
        {

        }
        public CosmosDbContext() { }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ProjectGraphic> Graphics { get; set; }
        public DbSet<ProjectTheme> Themes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity mappings and keys
            modelBuilder.Entity<Customer>().ToContainer("Customers");


            // Set partition key (if applicable)
            // modelBuilder.Entity<Customer>().HasPartitionKey(u => u.PartitionKey);

            base.OnModelCreating(modelBuilder);
        }
    }
}
