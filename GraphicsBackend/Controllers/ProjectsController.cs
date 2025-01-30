using System.Drawing;
using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using GraphicsBackend.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(string id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(_ => _.Id == id);
            if (project is null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewProject([FromBody] Project project)
        {
            try
            {
                project.Id = Guid.NewGuid().ToString("N")[..7].ToUpper();
                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();
                return Ok(project);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{projectId}")]
        public async Task<IActionResult> UpdateProject(string projectId, [FromBody] Project project)
        {
            try
            {
                var data = await _context.Projects.FirstOrDefaultAsync(_ => _.Id == projectId);
                if (data is null)
                {
                    return NotFound();
                }
                _context.Projects.Attach(project);
                await _context.SaveChangesAsync();
                return Ok(project);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


    }

}
