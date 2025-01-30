using GraphicsBackend.Contexts;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        //private readonly CosmosDbContext _cosmosDbContext;
        private readonly ApplicationDbContext _cosmosDbContext;

        public CustomersController(ApplicationDbContext context)
        {
            _cosmosDbContext = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var graphics = await _cosmosDbContext.ProjectGraphics.ToListAsync();
            var themes = await _cosmosDbContext.ProjectThemes.ToListAsync();
            await WebSocketHandler.NotifyClientsAsync($"Customers is now displayed.");
            return Ok(graphics);
        }



    }
}
