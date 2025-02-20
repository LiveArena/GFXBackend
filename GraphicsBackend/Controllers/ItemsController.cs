using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> GetProjectAsync(string id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500,ex.Message); 
            }
            
        }
        [HttpPost("projects")]
        public async Task<IActionResult> AddNewProjectAsync([FromBody] Project project)
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
                return StatusCode(500, ex.Message);
            }

        }
        [HttpPut("projects/{Id}")]
        public async Task<IActionResult> UpdateProjectAsync(string Id, [FromBody] Project project)
        {
            try
            {
                var data = await _context.Projects.FirstOrDefaultAsync(_ => _.Id == Id);
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

                return StatusCode(500,ex.Message);
            }

        }
        #endregion Project
        #region Theme
        [HttpGet("themes/{id}")]
        public async Task<IActionResult> GetThemeAsync(int id)
        {
            try
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
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

            
        }

        [HttpGet("themes/projects/{projectId}")]
        public async Task<IActionResult> GetThemesByProjectIdAsync(string projectId)
        {

            var Themes = await _context.ProjectThemes.Where(_ => _.ProjectId == projectId).ToListAsync();
            if (Themes is null)
            {
                return NotFound();
            }            
            return Ok(Themes);
        }

        [HttpPost("themes")]
        public async Task<IActionResult> AddNewThemeAsync([FromBody] ProjectTheme Theme)
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

                return StatusCode(500, ex.Message);

            }


        }

        [HttpPut("themes/{Id}")]
        public async Task<IActionResult> UpdateThemeByIdAsync(int Id, [FromBody] ProjectTheme theme)
        {
            try
            {
                var data = _context.ProjectThemes.FirstOrDefaultAsync(_ => _.Id == Id);
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

                return StatusCode(500, ex.Message);

            }


        }
        #endregion Theme
        #region Graphic
        [HttpGet("graphics/{id}")]
        public async Task<IActionResult> GetGraphicAsync(int id)
        {
            try
            {
                var cacheKey = $"Graphic_{id}";
                var cachedGraphic = await _cacheService.GetAsync<ProjectGraphic>(cacheKey);
                if (cachedGraphic != null)
                {
                    return Ok(cachedGraphic);
                }
                var graphic = await _context.ProjectGraphics.FirstOrDefaultAsync(_ => _.Id == id);
                if (graphic is null)
                {
                    return NotFound();
                }
                return Ok(graphic);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            

        }
        [HttpGet("graphics/projects/{projectId}")]
        public async Task<IActionResult> GetGraphicsByProjectIdAsync(string projectId)
        {

            var graphics = await _context.ProjectGraphics.Where(_ => _.ProjectId == projectId).ToListAsync();
            if (graphics is null)
            {
                return NotFound();
            }
            return Ok(graphics);

        }

        [HttpPost("graphics")]
        public async Task<IActionResult> AddNewGraphicAsync([FromBody] ProjectGraphic graphic)
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
                return StatusCode(500, ex.Message);
            }


        }

        [HttpPut("graphics/{Id}")]
        public async Task<IActionResult> UpdateGraphicByIdAsync(int Id, [FromBody] ProjectGraphic graphic)
        {
            try
            {
                var data = _context.ProjectGraphics.FirstOrDefaultAsync(_ => _.Id == Id);
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
                return StatusCode(500, ex.Message);
            }


        }
        #endregion Graphic

    }
}
