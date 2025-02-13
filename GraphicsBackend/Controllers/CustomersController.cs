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
        private readonly ApplicationDbContext _context;
        public CustomersController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var graphics = await _context.ProjectGraphics.ToListAsync();
            var themes = await _context.ProjectThemes.ToListAsync();
            string clientId = "";
            await WebSocketHandler.NotifyClientsAsync($"Customers is now displayed.", clientId);
            return Ok(graphics);
        }



    }
}
