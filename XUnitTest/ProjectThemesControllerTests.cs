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
        var project = new Project { Id = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00") };
        var theme1 = new ProjectTheme { Id = new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };
        var theme2 = new ProjectTheme { Id = new Guid("11223344-5566-7788-99P2-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };
        _context.Projects.Add(project);
        _context.ProjectThemes.AddRange(theme1, theme2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProjectThemeByIdAsync_ReturnsOkResult_WithTheme()
    {
        // Act
        var result = await _controller.GetProjectThemeByIdAsync(new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProjectTheme>(okResult.Value);
        Assert.Equal(new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), returnValue.Id);
    }

    [Fact]
    public async Task GetProjectThemeByIdAsync_ReturnsNotFound_WhenThemeNotFound()
    {
        // Act
        var result = await _controller.GetProjectThemeByIdAsync(new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProjectThemesByProjectIdAsync_ReturnsOkResult_WithThemes()
    {
        // Act
        var result = await _controller.GetProjectThemesByProjectIdAsync(new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<ProjectTheme>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
    }

    [Fact]
    public async Task GetProjectThemesByProjectIdAsync_ReturnsNotFound_WhenThemesNotFound()
    {
        // Act
        var result = await _controller.GetProjectThemesByProjectIdAsync(new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddThemeAsync_ReturnsOkResult_WithThemeId()
    {
        // Arrange
        var theme = new ProjectTheme { Id = new  Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };

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
        var theme = new ProjectTheme { Id = new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), theme);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ProjectTheme>(okResult.Value);
        Assert.Equal(new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), returnValue.Id);
    }

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsNotFound_WhenThemeNotFound()
    {
        // Arrange
        var theme = new ProjectTheme { Id = new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), theme);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProjectThemeByIdAsync_ReturnsBadRequest_OnException()
    {
        // Arrange
        var theme = new ProjectTheme { Id = new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };
        _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

        // Act
        var result = await _controller.UpdateProjectThemeByIdAsync(new Guid("11223344-5566-7788-99PT-BBCCDDEEFF00"), theme);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(badRequestResult.Value);
    }
}
