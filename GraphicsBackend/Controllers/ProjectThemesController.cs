using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using GraphicsBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectThemesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;        
        public ProjectThemesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectThemeByIdAsync(int id,CancellationToken cancellationToken)
        {
            var theme = await _context.ProjectThemes.FindAsync(id, cancellationToken);
            if (theme==null)
            {
                return NotFound();
            }
            return Ok(theme);
        }

        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectThemesByProjectIdAsync(string projectId,CancellationToken cancellationToken)
        {
            var projectThemes = await _context.ProjectThemes.Where(t => t.ProjectId == projectId).ToListAsync(cancellationToken);
            if (projectThemes.Count()==0)
            {
                return NotFound();
            }
            return Ok(projectThemes);
        }

        [HttpPost]
        public async Task<IActionResult> AddThemeAsync([FromBody] ProjectTheme Theme)
        {
            try
            {

                await _context.ProjectThemes.AddAsync(Theme);
                await _context.SaveChangesAsync();
                return Ok(Theme.Id);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }


        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateProjectThemeByIdAsync(int Id, [FromBody] ProjectTheme theme)
        {
            try
            {
                if (theme == null || Id != theme.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingTheme = await _context.ProjectThemes.FindAsync(Id);
                if (existingTheme == null)
                {
                    return NotFound();
                }

                // Detach the existing entity to avoid tracking conflicts
                _context.Entry(existingTheme).State = EntityState.Detached;

                // Attach and update the provided entity
                _context.ProjectThemes.Attach(theme);
                _context.Entry(theme).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                string clientId= "";
                await WebSocketHandler.NotifyClientsAsync(EnumSocketMessage.Theme_Updated.ToString(),clientId);

                return Ok(theme);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
