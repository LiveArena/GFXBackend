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
        public async Task<IActionResult> GetGraphic(int id)
        {

            var graphic = await _context.ProjectGraphics.FirstOrDefaultAsync(_ => _.Id == id);
            if (graphic is not null)
            {
                return Ok(graphic);
            }
            return NoContent();

        }
        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetGraphicsByProjectId(string projectId)
        {

            var graphics = await _context.ProjectGraphics.Where(g => g.ProjectId == projectId).ToListAsync();
            if (graphics is not null)
            {
                return Ok(graphics);
            }
            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> AddNewProjectGraphic([FromBody] ProjectGraphic graphic)
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
        public async Task<IActionResult> UpdateProjectGraphicById(int graphicId, [FromBody] ProjectGraphic graphic)
        {
            try
            {
                var existingGraphic = _context.ProjectGraphics.FirstOrDefaultAsync(_ => _.Id == graphicId);
                if (existingGraphic is null)
                {
                    return NotFound();
                }
                _context.ProjectGraphics.Attach(graphic);
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
        public async Task<IActionResult> HideAll(string projectId)
        {
            try
            {
                var existingGraphics = await _context.ProjectGraphics.ToListAsync();
                if (!existingGraphics.Any())
                {
                    return Ok(existingGraphics); // Return early if no graphics exist
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
        public async Task<IActionResult> Hide(int graphicId)
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

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }



    }

}
