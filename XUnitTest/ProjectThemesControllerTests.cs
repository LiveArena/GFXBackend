using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsBackend.Contexts;
using GraphicsBackend.Controllers;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProjectThemesControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly ProjectThemesController _controller;

    public ProjectThemesControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new ProjectThemesController(_context);

        // Seed the database with test data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var project = new Project { Id = Guid.NewGuid() };
        var theme1 = new ProjectTheme { Id = Guid.NewGuid(), ProjectId = project.Id, JSONData = "{}" };
        var theme2 = new ProjectTheme { Id = Guid.NewGuid(), ProjectId = project.Id, JSONData = "{}" };
        _context.Projects.Add(project);
        _context.ProjectThemes.AddRange(theme1, theme2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProjectThemeByIdAsync_ReturnsOkResult_WithTheme()
    {
        // Arrange
        var themeId = _context.ProjectThemes.First().Id;
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.GetProjectThemeByIdAsync(themeId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProjectTheme>(okResult.Value);
        Assert.Equal(themeId, returnValue.Id);
    }

    [Fact]
    public async Task GetProjectThemeByIdAsync_ReturnsNotFound_WhenThemeNotFound()
    {
        // Act
        var result = await _controller.GetProjectThemeByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProjectThemesByProjectIdAsync_ReturnsOkResult_WithThemes()
    {
        // Arrange
        var projectId = _context.Projects.First().Id;
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.GetProjectThemesByProjectIdAsync(projectId, cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProjectTheme>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetProjectThemesByProjectIdAsync_ReturnsNotFound_WhenThemesNotFound()
    {
        // Act
        var result = await _controller.GetProjectThemesByProjectIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddThemeAsync_ReturnsOkResult_WithThemeId()
    {
        // Arrange
        var theme = new ProjectTheme { Id = Guid.NewGuid(), ProjectId = _context.Projects.First().Id, JSONData = "{}" };

        // Act
        var result = await _controller.AddThemeAsync(theme);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<Guid>(okResult.Value);
        Assert.Equal(theme.Id, returnValue);
    }

    

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsOkResult_WithTheme()
    {
        // Arrange
        var themeId = _context.ProjectThemes.First().Id;
        var theme = new ProjectTheme { Id = themeId, ProjectId = _context.Projects.First().Id, JSONData = "{}" };

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(themeId, theme);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProjectTheme>(okResult.Value);
        Assert.Equal(themeId, returnValue.Id);
    }

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsNotFound_WhenThemeNotFound()
    {
        // Arrange
        var themeId = Guid.NewGuid();
        var theme = new ProjectTheme { Id = themeId, ProjectId = _context.Projects.First().Id, JSONData = "{}" };

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(themeId, theme);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsBadRequest_OnException()
    {
        // Arrange
        var themeId = _context.ProjectThemes.First().Id;
        var theme = new ProjectTheme { Id = themeId, ProjectId = _context.Projects.First().Id, JSONData = "{}" };
        _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(Guid.NewGuid(), theme);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}
