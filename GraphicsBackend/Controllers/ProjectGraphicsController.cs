using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using GraphicsBackend.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IActionResult> GetProjectGraphicByIdAsync(int id,CancellationToken cancellationToken)
        {

            var graphic = await _context.ProjectGraphics.FindAsync(id,cancellationToken);
            if (graphic is not null)
            {
                return Ok(graphic);
            }
            return NoContent();

        }
        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectGraphicsByProjectIdAsync(string projectId,CancellationToken cancellationToken)
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

                await _context.ProjectGraphics.AddAsync(graphic);
                await _context.SaveChangesAsync();
                await WebSocketHandler.NotifyClientsAsync(EnumSocketMessage.Graphic_Created.ToString());
                return Ok(graphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut("{graphicId}")]
        public async Task<IActionResult> UpdateProjectGraphicByIdAsync(int graphicId, [FromBody] ProjectGraphic graphic)
        {
            try
            {
                if (graphic == null || graphicId != graphic.Id)
                {
                    return BadRequest("Invalid graphic data.");
                }

                var existingGraphic = await _context.ProjectGraphics.FindAsync(graphicId);
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
                await WebSocketHandler.NotifyClientsAsync(EnumSocketMessage.Graphic_Updated.ToString());

                return Ok(graphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        [HttpPut("hideUnhideAll/{projectId}")]
        public async Task<IActionResult> HideUnhideAllAsync(string projectId, CancellationToken cancellationToken)
        {
            try
            {
                var existingGraphics = await _context.ProjectGraphics.Where(pg => pg.ProjectId == projectId).ToListAsync(cancellationToken);             
                if (existingGraphics.Count == 0)
                {
                    return Ok(existingGraphics);
                }

                existingGraphics.ForEach(graphic => graphic.Hide = !graphic.Hide); // Toggle Hide property

                await _context.SaveChangesAsync();
                await WebSocketHandler.NotifyClientsAsync(EnumSocketMessage.Graphic_Updated.ToString());

                return Ok(existingGraphics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("hideUnhide/{graphicId}")]
        public async Task<IActionResult> HideAsync(int graphicId)
        {
            try
            {
               
                var existingGraphic = await _context.ProjectGraphics.FindAsync(graphicId);
                if (existingGraphic == null)
                {
                    return NotFound();
                }

                // Toggle Hide property
                existingGraphic.Hide = !existingGraphic.Hide;

                await _context.SaveChangesAsync();
                await WebSocketHandler.NotifyClientsAsync(EnumSocketMessage.Graphic_Updated.ToString());

                return Ok(existingGraphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



    }

}
