using GraphicsBackend.Contexts;
using GraphicsBackend.Controllers;
using GraphicsBackend.DTOs;
using GraphicsBackend.Models;
using GraphicsBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraphicsBackend.XUnitTests
{
    public class GraphicsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GraphicsController _controller;
        private readonly IWebSocketService _webSocketService;

        public GraphicsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _webSocketService=new WebSocketService();
            _controller = new GraphicsController(_context,_webSocketService);

            // Seed the database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var project = new Project { Id = Guid.NewGuid() };
            var graphic1 = new ProjectGraphic { Id = Guid.NewGuid(), ProjectId = project.Id, JSONData = "{}" };
            var graphic2 = new ProjectGraphic { Id = Guid.NewGuid(), ProjectId = project.Id, JSONData = "{}" };
            _context.Projects.Add(project);
            _context.ProjectGraphics.AddRange(graphic1, graphic2);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProjectGraphicByIdAsync_ReturnsOkResult_WithGraphic()
        {
            // Arrange
            var graphicId = _context.ProjectGraphics.First().Id;
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _controller.GetProjectGraphicByIdAsync(graphicId, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(graphicId, returnValue.Id);
        }

        [Fact]
        public async Task GetProjectGraphicByIdAsync_ReturnsNoContent_WhenGraphicNotFound()
        {
            // Act
            var result = await _controller.GetProjectGraphicByIdAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetProjectGraphicsByProjectIdAsync_ReturnsOkResult_WithGraphics()
        {
            // Arrange
            var projectId = _context.Projects.First().Id;
            var cancellationToken = new CancellationToken();

            // Act
            var result = await _controller.GetProjectGraphicsByProjectIdAsync(projectId, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectGraphic>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetProjectGraphicsByProjectIdAsync_ReturnsNoContent_WhenGraphicsNotFound()
        {
            // Act
            var result = await _controller.GetProjectGraphicsByProjectIdAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddProjectGraphicAsync_ReturnsOkResult_WithGraphic()
        {
            // Arrange
            var graphic = new GraphicDTO { Id = Guid.NewGuid(), ProjectId = _context.Projects.First().Id, JSONData = "{}" };

            // Act
            var result = await _controller.AddProjectGraphicAsync(graphic);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(graphic.Id, returnValue.Id);
        }
        

        [Fact]
        public async Task UpdateProjectGraphicByIdAsync_ReturnsOkResult_WithGraphic()
        {
            // Arrange
            var graphicId = _context.ProjectGraphics.First().Id;
            var graphic = new GraphicDTO { Id = graphicId, ProjectId = _context.Projects.First().Id, JSONData = "{}" };

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(graphicId, graphic);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(graphicId, returnValue.Id);
        }

        [Fact]
        public async Task UpdateProjectGraphicByIdAsync_ReturnsNotFound_WhenGraphicNotFound()
        {
            // Arrange
            var graphicId = Guid.NewGuid();
            var graphic = new GraphicDTO { Id = graphicId, ProjectId = _context.Projects.First().Id, JSONData = "{}" };

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(graphicId, graphic);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProjectGraphicByIdAsync_ReturnsBadRequest_OnException()
        {
            // Arrange
            var graphicId = _context.ProjectGraphics.First().Id;
            var graphic = new GraphicDTO { Id = graphicId, ProjectId = _context.Projects.First().Id, JSONData = "{}" };
            _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(Guid.NewGuid(), graphic);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }
    }
}