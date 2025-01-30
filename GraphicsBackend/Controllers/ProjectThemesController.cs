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
        public async Task<IActionResult> GetTheme(int id)
        {
            var theme = await _context.ProjectThemes.FirstOrDefaultAsync(_ => _.Id == id);
            if (theme is null)
            {
                return NotFound();
            }
            return Ok(theme);
        }

        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectThemesByProjectId(string projectId)
        {
            var ProjectThemes = await _context.ProjectThemes.Where(t => t.ProjectId == projectId).ToListAsync();
            if (ProjectThemes == null)
            {
                return NotFound();
            }
            return Ok(ProjectThemes);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewTheme([FromBody] ProjectTheme Theme)
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
        public async Task<IActionResult> UpdateProjectThemeById(int projectThemeId, [FromBody] ProjectTheme theme)
        {
            try
            {
                var existingTheme = _context.ProjectThemes.FirstOrDefaultAsync(_ => _.Id == projectThemeId);
                if (existingTheme is null)
                {
                    return NotFound();
                }
                _context.ProjectThemes.Attach(theme);
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
