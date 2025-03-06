using GraphicsBackend.Contexts;
using GraphicsBackend.Controllers;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class ProjectsControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly ProjectsController _controller;
    private readonly IWebSocketService _webSocketService;
    public ProjectsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _webSocketService=new WebSocketService();
        _controller = new ProjectsController(_context,_webSocketService);

        // Seed the database with test data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var project = new Project { Id = Guid.NewGuid(), CustomerId = 1 };
        _context.Projects.Add(project);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsOkResult_WithProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, CustomerId = 1 };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.GetProjectByIdAsync(projectId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Project>(okResult.Value);
        Assert.Equal(projectId, returnValue.Id);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsNotFound_WhenProjectNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.GetProjectByIdAsync(projectId, cancellationToken);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddProjectAsync_ReturnsOkResult_WithProject()
    {
        // Arrange
        var project = new Project { Id = Guid.NewGuid(), CustomerId = 1 };

        // Act
        var result = await _controller.AddProjectAsync(project);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Project>(okResult.Value);
        Assert.Equal(project.Id, returnValue.Id);
    }
    

    [Fact]
    public async Task UpdateProjectAsync_ReturnsOkResult_WithUpdatedProject()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, CustomerId = 1 };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();

        var updatedProject = new Project { Id = projectId, CustomerId = 2 };

        // Act
        var result = await _controller.UpdateProjectAsync(projectId, updatedProject);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Project>(okResult.Value);
        Assert.Equal(2, returnValue.CustomerId);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsNotFound_WhenProjectNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, CustomerId = 1 };

        // Act
        var result = await _controller.UpdateProjectAsync(projectId, project);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsBadRequest_OnException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project { Id = projectId, CustomerId = 1 };
        _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

        // Act
        var result = await _controller.UpdateProjectAsync(Guid.NewGuid(), project);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}
