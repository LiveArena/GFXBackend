using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRedisCacheService _cacheService;
        public ItemsController(ApplicationDbContext context, IRedisCacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }
        #region Project
        [HttpGet("projects/{id}")]
        public async Task<IActionResult> GetProject(string id)
        {
            var cacheKey = $"Project_{id}";
            var cachedProject = await _cacheService.GetAsync<Project>(cacheKey);
            if (cachedProject != null)
            {
                return Ok(cachedProject);
            }
            var project = await _context.Projects.FirstOrDefaultAsync(_ => _.Id == id);
            if (project is null)
            {
                return NotFound();
            }
            
            return Ok(project);
        }
        [HttpPost("projects")]
        public async Task<IActionResult> AddNewProject([FromBody] Project project)
        {

            try
            {


                var cacheKey = $"Project_{project.Id}";
                await _cacheService.SetAsync(cacheKey, project, TimeSpan.FromMinutes(10));
                await _context.Projects.AddAsync(project);
                await _context.SaveChangesAsync();               
                return Ok(project);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPut("projects/{projectId}")]
        public async Task<IActionResult> UpdateProject(string projectId, [FromBody] Project project)
        {
            try
            {
                var data = await _context.Projects.FirstOrDefaultAsync(_ => _.Id == projectId);
                if (data is null)
                {
                    return NotFound();
                }
                _context.Projects.Attach(project);
                await _context.SaveChangesAsync();                
                return Ok(project);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        #endregion Project
        #region Theme
        [HttpGet("themes/{id}")]
        public async Task<IActionResult> GetTheme(int id)
        {
            var cacheKey = $"Theme_{id}";
            var cachedTheme = await _cacheService.GetAsync<ProjectTheme>(cacheKey);
            if (cachedTheme != null)
            {
                return Ok(cachedTheme);
            }
            var Theme = await _context.ProjectThemes.FirstOrDefaultAsync(_ => _.Id == id);
            if (Theme is null)
            {
                return NotFound();
            }            
            return Ok(Theme);
        }

        [HttpGet("themes/projects/{projectId}")]
        public async Task<IActionResult> GetThemesByProjectId(string projectId)
        {

            var Themes = await _context.ProjectThemes.Where(_ => _.ProjectId == projectId).ToListAsync();
            if (Themes is null)
            {
                return NotFound();
            }            
            return Ok(Themes);
        }

        [HttpPost("themes")]
        public async Task<IActionResult> AddNewTheme([FromBody] ProjectTheme Theme)
        {
            try
            {

                var cacheKey = $"Theme_{Theme.Id}";
                await _cacheService.SetAsync(cacheKey, Theme, TimeSpan.FromMinutes(10));
                await _context.ProjectThemes.AddAsync(Theme);
                await _context.SaveChangesAsync();                
                return Ok(Theme.Id);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }


        }

        [HttpPut("themes/{themeId}")]
        public async Task<IActionResult> UpdateThemeById(int themeId, [FromBody] ProjectTheme theme)
        {
            try
            {
                var data = _context.ProjectThemes.FirstOrDefaultAsync(_ => _.Id == themeId);
                if (data is null)
                {
                    return NotFound();
                }
                _context.ProjectThemes.Attach(theme);
                await _context.SaveChangesAsync();                
                return Ok(theme);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);

            }


        }
        #endregion Theme
        #region Graphic
        [HttpGet("graphics/{id}")]
        public async Task<IActionResult> GetGraphic(int id)
        {
            var cacheKey = $"Graphic_{id}";
            var cachedGraphic = await _cacheService.GetAsync<ProjectGraphic>(cacheKey);
            if (cachedGraphic != null)
            {
                return Ok(cachedGraphic);
            }
            var graphic = await _context.ProjectGraphics.FirstOrDefaultAsync(_ => _.Id == id);
            if (graphic is not null)
            {
                return Ok(graphic);
            }
            return NoContent();

        }
        [HttpGet("graphics/projects/{projectId}")]
        public async Task<IActionResult> GetGraphicsByProjectId(string projectId)
        {

            var graphics = await _context.ProjectGraphics.Where(_ => _.ProjectId == projectId).ToListAsync();
            if (graphics is not null)
            {
                return Ok(graphics);
            }
            return NoContent();

        }

        [HttpPost("graphics")]
        public async Task<IActionResult> AddNewGraphic([FromBody] ProjectGraphic graphic)
        {
            try
            {

                var cacheKey = $"Graphic_{graphic.Id}";
                await _cacheService.SetAsync(cacheKey, graphic, TimeSpan.FromMinutes(10));
                await _context.ProjectGraphics.AddAsync(graphic);
                await _context.SaveChangesAsync();
                return Ok(graphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPut("graphics/{graphicId}")]
        public async Task<IActionResult> UpdateGraphicById(int graphicId, [FromBody] ProjectGraphic graphic)
        {
            try
            {
                var data = _context.ProjectGraphics.FirstOrDefaultAsync(_ => _.Id == graphicId);
                if (data is null)
                {
                    return NotFound();
                }
                _context.ProjectGraphics.Attach(graphic);
                await _context.SaveChangesAsync();
                return Ok(graphic);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }
        #endregion Graphic

    }
}
