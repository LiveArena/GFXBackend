using GraphicsBackend.Contexts;
using GraphicsBackend.Controllers;
using GraphicsBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.XUnitTests
{
    public class ProjectGraphicsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ProjectGraphicsController _controller;

        public ProjectGraphicsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ProjectGraphicsController(_context);

            // Seed the database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var project = new Project { Id = "1234567" };
            var graphic = new ProjectGraphic { Id = 1, ProjectId = "1234567", JSONData = "{}", Hide = false };
            _context.Projects.Add(project);
            _context.ProjectGraphics.Add(graphic);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProjectGraphicByIdAsync_ReturnsOkResult_WithGraphic()
        {
            // Act
            var result = await _controller.GetProjectGraphicByIdAsync(1, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task GetProjectGraphicByIdAsync_ReturnsNoContent_WhenGraphicNotFound()
        {
            // Act
            var result = await _controller.GetProjectGraphicByIdAsync(999, CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetProjectGraphicsByProjectIdAsync_ReturnsOkResult_WithGraphics()
        {
            // Act
            var result = await _controller.GetProjectGraphicsByProjectIdAsync("1234567", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectGraphic>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetProjectGraphicsByProjectIdAsync_ReturnsNoContent_WhenGraphicsNotFound()
        {
            // Act
            var result = await _controller.GetProjectGraphicsByProjectIdAsync("nonexistent", CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddProjectGraphicAsync_ReturnsOkResult_WithGraphic()
        {
            // Arrange
            var graphic = new ProjectGraphic { ProjectId = "1234567", JSONData = "{}", Hide = false };

            // Act
            var result = await _controller.AddProjectGraphicAsync(graphic);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.NotNull(returnValue.Id);
        }

        [Fact]
        public async Task AddProjectGraphicAsync_ReturnsBadRequest_OnException()
        {
            // Arrange
            var graphic = new ProjectGraphic { ProjectId = "1234567", JSONData = "{}", Hide = false };
            _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

            // Act
            var result = await _controller.AddProjectGraphicAsync(graphic);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateProjectGraphicByIdAsync_ReturnsOkResult_WithGraphic()
        {
            // Arrange
            var graphic = new ProjectGraphic { Id = 1, ProjectId = "1234567", JSONData = "{}", Hide = false };

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(1, graphic);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task UpdateProjectGraphicById_ReturnsNotFound_WhenGraphicNotFound()
        {
            // Arrange
            var graphic = new ProjectGraphic { Id = 999, ProjectId = "1234567", JSONData = "{}", Hide = false };

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(999, graphic);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProjectGraphicByIdAsync_ReturnsBadRequest_OnException()
        {
            // Arrange
            var graphic = new ProjectGraphic { Id = 1, ProjectId = "1234567", JSONData = "{}", Hide = false };
            _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(1, graphic);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task HideUnhideAllAsync_ReturnsOkResult_WithUpdatedGraphics()
        {
            // Act
            var result = await _controller.HideUnhideAllAsync("1234567", CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectGraphic>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.True(returnValue.First().Hide);
        }

        

        [Fact]
        public async Task HideAsync_ReturnsOkResult_WithUpdatedGraphic()
        {
            // Act
            var result = await _controller.HideAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.True(returnValue.Hide);
        }

        [Fact]
        public async Task HideAsync_ReturnsNotFound_WhenGraphicNotFound()
        {
            // Act
            var result = await _controller.HideAsync(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task HideAsync_ReturnsBadRequest_OnException()
        {
            // Arrange
            _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

            // Act
            var result = await _controller.HideAsync(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}