using GraphicsBackend.Contexts;
using GraphicsBackend.Controllers;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class ProjectsControllerTests
{
    private readonly ApplicationDbContext _mockContext;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _mockContext = new ApplicationDbContext(options);
        _controller = new ProjectsController(_mockContext);
        SeedDatabase();
    }
    private void SeedDatabase()
    {
        var project = new Project { Id = "1234567" };
        _mockContext.Projects.Add(project);
        _mockContext.SaveChanges();
    }    

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsOkResult_WithProject()
    {

        //// Arrange
        var projectId = "1234567";
        // Act
        var result = await _controller.GetProjectByIdAsync(projectId,CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Project>(okResult.Value);
        Assert.Equal(projectId, returnValue.Id);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsNotFound_WhenProjectNotFound()
    {
        // Arrange
        var projectId = "nonexistent";
        // Act
        var result = await _controller.GetProjectByIdAsync(projectId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddProjectAsync_ReturnsOkResult_WithProject()
    {
        var project = new Project { Id = "2345678" };

        // Act
        var result = await _controller.AddProjectAsync(project);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Project>(okResult.Value);
        Assert.NotNull(returnValue.Id);
        Assert.Equal(7, returnValue.Id.Length);
    }    

    [Fact]
    public async Task UpdateProjectAsync_ReturnsOkResult_WithProject()
    {
        // Arrange
        var project = new Project { Id = "1234567" };

        // Act
        var result = await _controller.UpdateProjectAsync("1234567", project);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Project>(okResult.Value);
        Assert.Equal("1234567", returnValue.Id);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsNotFound_WhenProjectNotFound()
    {
        // Arrange
        var project = new Project { Id = "nonexistent" };

        // Act
        var result = await _controller.UpdateProjectAsync("nonexistent", project);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProjectAsync_ReturnsBadRequest_OnException()
    {
        // Arrange
        var project = new Project { Id = "1234567" };
        var res= _mockContext.Database.EnsureDeleted(); // Simulate an exception by deleting the database

        // Act
        var result = await _controller.UpdateProjectAsync("1234567", project);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}