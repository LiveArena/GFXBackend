using GraphicsBackend.Contexts;
using GraphicsBackend.DTOs;
using GraphicsBackend.Models;
using GraphicsBackend.Notifications;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Controllers
{
    [Authorize]
    public class GraphicsController : WebSocketSupportController
    {
        private readonly ApplicationDbContext _context;

        protected override SocketMessageType SocketMessageType => SocketMessageType.Graphic;

        public GraphicsController(ApplicationDbContext context, IWebSocketService webSocketService)
            :base(webSocketService)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectGraphicByIdAsync(Guid id,CancellationToken cancellationToken)
        {

            var graphic = await _context.ProjectGraphics.FindAsync(id,cancellationToken);
            if (graphic is not null)
            {
                BroadcastThroughSocket(ActionTaken.ReadSingle, graphic);

                return Ok(graphic);
            }
            return NoContent();

        }
        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectGraphicsByProjectIdAsync(Guid projectId,CancellationToken cancellationToken)
        {

            var graphics = await _context.ProjectGraphics.Where(g => g.ProjectId == projectId).ToListAsync(cancellationToken);
            if (graphics.Count != 0)
            {
                BroadcastThroughSocket(ActionTaken.ReadList, graphics);

                return Ok(graphics);
            }
            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> AddProjectGraphicAsync([FromBody] GraphicDTO graphic)
        {
            try
            {
                var response = graphic.Create();
                await _context.ProjectGraphics.AddAsync(response);
                await _context.SaveChangesAsync();

                BroadcastThroughSocket(ActionTaken.Created, response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProjectGraphicByIdAsync(Guid Id, [FromBody] GraphicDTO graphic)
        {
            try
            {
                if (graphic == null || Id != graphic.Id)
                {
                    return BadRequest("Invalid graphic data.");
                }

                var existingGraphic = await _context.ProjectGraphics.FindAsync(Id);
                if (existingGraphic is null) 
                {
                    return NotFound();
                }

                existingGraphic.ProjectId = graphic.ProjectId;
                existingGraphic.JSONData = graphic.JSONData;
                _context.ProjectGraphics.Attach(existingGraphic);
                await _context.SaveChangesAsync();

                BroadcastThroughSocket(ActionTaken.Updated, graphic);
                
                return Ok(existingGraphic);                   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveGraphicById(Guid id)
        {
            try
            {
                var graphicToDelete = await _context.ProjectGraphics.FindAsync(id);
                if (graphicToDelete is null)
                {
                    return NotFound($"Graphic not found with id='{id}'");
                }

                _context.ProjectGraphics.Remove(graphicToDelete);
                await _context.SaveChangesAsync();

                BroadcastThroughSocket(ActionTaken.Deleted, id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}
