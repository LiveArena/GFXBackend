using GraphicsBackend.Contexts;
using GraphicsBackend.Services;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend
{
    public static class ServiceExtensions
    {
        public static void MigrateDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
