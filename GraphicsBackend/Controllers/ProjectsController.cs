using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
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
        public async Task<IActionResult> GetProjectByIdAsync(string id,CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(id,cancellationToken);
            if (project is null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        [HttpPost]
        public async Task<IActionResult> AddProjectAsync([FromBody] Project project)
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
        public async Task<IActionResult> UpdateProjectAsync(string projectId, [FromBody] Project project)
        {
            try
            {
                if (project == null || projectId != project.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingProject = await _context.Projects.FindAsync(projectId);
                if (existingProject == null)
                {
                    return NotFound();
                }

                // Detach the existing entity to avoid tracking conflicts
                _context.Entry(existingProject).State = EntityState.Detached;

                // Attach and update the provided entity
                _context.Projects.Attach(project);
                _context.Entry(project).State = EntityState.Modified;
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
