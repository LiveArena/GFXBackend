using GraphicsBackend.Contexts;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;        
        public ItemsController(ApplicationDbContext context)
        {
            _context = context;            
        }
        #region Project
        [HttpGet("projects/{id}")]
        public async Task<IActionResult> GetProjectAsync(Guid id)
        {
            try
            {                
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
                project.Id = Guid.NewGuid();
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
        public async Task<IActionResult> UpdateProjectAsync(Guid Id, [FromBody] Project project)
        {
            try
            {
                var data = await _context.Projects.AnyAsync(_ => _.Id == Id);
                if (!data )
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
        public async Task<IActionResult> GetThemeAsync(Guid id)
        {
            try
            {
                
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
        public async Task<IActionResult> GetThemesByProjectIdAsync(Guid projectId)
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
                Theme.Id = Guid.NewGuid();
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
        public async Task<IActionResult> UpdateThemeByIdAsync(Guid Id, [FromBody] ProjectTheme theme)
        {
            try
            {
                var data =await _context.ProjectThemes.AnyAsync(_ => _.Id == Id);
                if (!data )
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
        public async Task<IActionResult> GetGraphicAsync(Guid id)
        {
            try
            {
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
        public async Task<IActionResult> GetGraphicsByProjectIdAsync(Guid projectId)
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
                graphic.Id = Guid.NewGuid();
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
        public async Task<IActionResult> UpdateGraphicByIdAsync(Guid Id, [FromBody] ProjectGraphic graphic)
        {
            try
            {
                var data = await _context.ProjectGraphics.AnyAsync(_ => _.Id == Id);
                if (!data)
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
        [HttpDelete("graphics/{id}")]
        public async Task<IActionResult> RemoveGraphicByIdAsync(Guid id)
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
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion Graphic

    }
}
