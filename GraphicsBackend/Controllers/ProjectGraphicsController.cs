using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]   
    public class ProjectGraphicsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProjectGraphicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectGraphicByIdAsync(Guid id,CancellationToken cancellationToken)
        {

            var graphic = await _context.ProjectGraphics.FindAsync(id,cancellationToken);
            if (graphic is not null)
            {
                return Ok(graphic);
            }
            return NoContent();

        }
        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectGraphicsByProjectIdAsync(Guid projectId,CancellationToken cancellationToken)
        {

            var graphics = await _context.ProjectGraphics.Where(g => g.ProjectId == projectId).ToListAsync(cancellationToken);
            if (graphics.Any())
            {
                return Ok(graphics);
            }
            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> AddProjectGraphicAsync([FromBody] ProjectGraphic graphic)
        {
            try
            {
                graphic.Id = Guid.NewGuid();
                await _context.ProjectGraphics.AddAsync(graphic);
                await _context.SaveChangesAsync();                
                return Ok(graphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProjectGraphicByIdAsync(Guid Id, [FromBody] ProjectGraphic graphic)
        {
            try
            {
                if (graphic == null || Id != graphic.Id)
                {
                    return BadRequest("Invalid graphic data.");
                }

                var existingGraphic = await _context.ProjectGraphics.FindAsync(Id);
                if (existingGraphic == null)
                {
                    return NotFound();
                }

                // Detach the existing entity to avoid tracking conflicts
                _context.Entry(existingGraphic).State = EntityState.Detached;

                // Attach and update the provided entity
                _context.ProjectGraphics.Attach(graphic);
                _context.Entry(graphic).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return Ok(graphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        



    }

}
