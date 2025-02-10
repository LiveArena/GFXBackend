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
        var project = new Project { Id = "1234567" };
        var theme1 = new ProjectTheme { Id = 1, ProjectId = "1234567", JSONData = "{}" };
        var theme2 = new ProjectTheme { Id = 2, ProjectId = "1234567", JSONData = "{}" };
        _context.Projects.Add(project);
        _context.ProjectThemes.AddRange(theme1, theme2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProjectThemeByIdAsync_ReturnsOkResult_WithTheme()
    {
        // Act
        var result = await _controller.GetProjectThemeByIdAsync(1, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProjectTheme>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetProjectThemeByIdAsync_ReturnsNotFound_WhenThemeNotFound()
    {
        // Act
        var result = await _controller.GetProjectThemeByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProjectThemesByProjectIdAsync_ReturnsOkResult_WithThemes()
    {
        // Act
        var result = await _controller.GetProjectThemesByProjectIdAsync("1234567", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProjectTheme>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetProjectThemesByProjectIdAsync_ReturnsNotFound_WhenThemesNotFound()
    {
        // Act
        var result = await _controller.GetProjectThemesByProjectIdAsync("nonexistent", CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddThemeAsync_ReturnsOkResult_WithThemeId()
    {
        // Arrange
        var theme = new ProjectTheme { Id = 3, ProjectId = "1234567", JSONData = "{}" };

        // Act
        var result = await _controller.AddThemeAsync(theme);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<int>(okResult.Value);
        Assert.True(returnValue > 0);
    }

    
    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsOkResult_WithTheme()
    {
        // Arrange
        var theme = new ProjectTheme { Id = 1, ProjectId = "1234567", JSONData = "{}" };

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(1, theme);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProjectTheme>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsNotFound_WhenThemeNotFound()
    {
        // Arrange
        var theme = new ProjectTheme { Id = 999, ProjectId = "1234567", JSONData = "{}" };

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(999, theme);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsBadRequest_OnException()
    {
        // Arrange
        var theme = new ProjectTheme { Id = 1, ProjectId = "1234567", JSONData = "{}" };
        _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(1, theme);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}
