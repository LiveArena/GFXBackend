using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using GraphicsBackend.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty ProjectThemes, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var ProjectThemes = await _context.ProjectThemes.Where(t => t.ProjectId == projectId).ToListAsync(cancellationToken);
            if (ProjectThemes.Count()==0)
            {
                return NotFound();
            }
            return Ok(ProjectThemes);
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

        [HttpPut("{projectThemeId}")]
        public async Task<IActionResult> UpdateProjectThemeByIdAsync(int projectThemeId, [FromBody] ProjectTheme theme)
        {
            try
            {
                if (theme == null || projectThemeId != theme.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingTheme = await _context.ProjectThemes.FindAsync(projectThemeId);
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
                await WebSocketHandler.NotifyClientsAsync(EnumSocketMessage.Theme_Updated.ToString());

                return Ok(theme);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
