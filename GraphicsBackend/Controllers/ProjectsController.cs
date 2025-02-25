using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectByIdAsync(Guid id,CancellationToken cancellationToken)
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
                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();
                return Ok(project);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProjectAsync(Guid Id, [FromBody] Project project)
        {
            try
            {
                if (project == null || Id != project.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingProject = await _context.Projects.FindAsync(Id);
                if (existingProject is not null)
                {
                    existingProject.CustomerId = project.CustomerId;
                    _context.Projects.Attach(existingProject);
                    await _context.SaveChangesAsync();
                    return Ok(existingProject);                    
                }
                return NotFound();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


    }

}
