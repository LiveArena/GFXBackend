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
    public class ThemesController : WebSocketSupportController
    {
        private readonly ApplicationDbContext _context;

        protected override SocketMessageType SocketMessageType => SocketMessageType.Theme;

        public ThemesController(ApplicationDbContext context, IWebSocketService webSocketService)
            : base(webSocketService)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectThemeByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var theme = await _context.ProjectThemes.FindAsync(id, cancellationToken);
            if (theme == null)
            {
                return NotFound();
            }

            BroadcastThroughSocket(ActionTaken.ReadSingle, theme);
            return Ok(theme);
        }

        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectThemesByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
        {
            var projectThemes = await _context.ProjectThemes.Where(t => t.ProjectId == projectId).ToListAsync(cancellationToken);
            if (projectThemes.Count == 0)
            {
                return NotFound();
            }

            BroadcastThroughSocket(ActionTaken.ReadList, projectThemes);
            return Ok(projectThemes);
        }

        [HttpPost]
        public async Task<IActionResult> AddThemeAsync([FromBody] ThemeDTO theme)
        {
            try
            {
                var response = theme.Create();
                await _context.ProjectThemes.AddAsync(response);
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
        public async Task<IActionResult> UpdateProjectThemeByIdAsync(Guid Id, [FromBody] ThemeDTO theme)
        {
            try
            {
                if (theme == null || Id != theme.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingTheme = await _context.ProjectThemes.FindAsync(Id);
                if (existingTheme is null)
                {
                    return NotFound();
                }


                existingTheme.ProjectId = theme.ProjectId;
                existingTheme.JSONData = theme.JSONData;
                _context.ProjectThemes.Attach(existingTheme);
                await _context.SaveChangesAsync();

                BroadcastThroughSocket(ActionTaken.Updated, existingTheme);
                return Ok(existingTheme);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
