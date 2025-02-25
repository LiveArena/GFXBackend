using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetProjectThemeByIdAsync(Guid id,CancellationToken cancellationToken)
        {
            var theme = await _context.ProjectThemes.FindAsync(id, cancellationToken);
            if (theme==null)
            {
                return NotFound();
            }
            return Ok(theme);
        }

        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectThemesByProjectIdAsync(Guid projectId,CancellationToken cancellationToken)
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
        public async Task<IActionResult> UpdateProjectThemeByIdAsync(Guid Id, [FromBody] ProjectTheme theme)
        {
            try
            {
                if (theme == null || Id != theme.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingTheme = await _context.ProjectThemes.FindAsync(Id);
                if (existingTheme is not null)
                {
                    existingTheme.ProjectId = theme.ProjectId;
                    existingTheme.JSONData = theme.JSONData;
                    _context.ProjectThemes.Attach(existingTheme);
                    await _context.SaveChangesAsync();
                    return Ok(existingTheme);
                    
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
