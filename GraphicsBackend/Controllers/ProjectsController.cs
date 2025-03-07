using GraphicsBackend.Contexts;
using GraphicsBackend.DTOs;
using GraphicsBackend.Models;
using GraphicsBackend.Notifications;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Mvc;


namespace GraphicsBackend.Controllers
{  
    public class ProjectsController : WebSocketSupportController
    {
        private readonly ApplicationDbContext _context;

        protected override SocketMessageType SocketMessageType => SocketMessageType.Project;

        public ProjectsController(ApplicationDbContext context, IWebSocketService websocketService)
            :base(websocketService)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectByIdAsync(Guid id,CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(id,cancellationToken);
            if (project is null)
            {
                return NotFound();
            }

            BroadcastThroughSocket(ActionTaken.ReadSingle, project);
            return Ok(project);
        }
        [HttpPost]
        public async Task<IActionResult> AddProjectAsync([FromBody] ProjectDTO projectDto)
        {
            try
            {
                var response = projectDto.Create();
                ////await _context.Projects.AddAsync(response);
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
        public async Task<IActionResult> UpdateProjectAsync(Guid Id, [FromBody] ProjectDTO project)
        {
            try
            {
                if (project == null || Id != project.Id)
                {
                    return BadRequest("Invalid theme data.");
                }

                var existingProject = await _context.Projects.FindAsync(Id);
                if (existingProject is null) 
                {
                    return NotFound();
                }

                existingProject.JSONData = project.JSONData;
                existingProject.CustomerId = project.CustomerId;
                _context.Projects.Attach(existingProject);
                await _context.SaveChangesAsync();

                BroadcastThroughSocket(ActionTaken.Updated, existingProject);
                return Ok(existingProject);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


    }

}
