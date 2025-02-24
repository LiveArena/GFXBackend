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
            var project = new Project { Id = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00") };
            var graphic = new ProjectGraphic { Id = new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };
            _context.Projects.Add(project);
            _context.ProjectGraphics.Add(graphic);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetProjectGraphicByIdAsync_ReturnsOkResult_WithGraphic()
        {
            // Act
            var result = await _controller.GetProjectGraphicByIdAsync(new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), returnValue.Id);
        }

        [Fact]
        public async Task GetProjectGraphicByIdAsync_ReturnsNoContent_WhenGraphicNotFound()
        {
            // Act
            var result = await _controller.GetProjectGraphicByIdAsync(new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetProjectGraphicsByProjectIdAsync_ReturnsOkResult_WithGraphics()
        {
            // Act
            var result = await _controller.GetProjectGraphicsByProjectIdAsync(new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ProjectGraphic>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetProjectGraphicsByProjectIdAsync_ReturnsNoContent_WhenGraphicsNotFound()
        {
            // Act
            var result = await _controller.GetProjectGraphicsByProjectIdAsync(new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), CancellationToken.None);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task AddProjectGraphicAsync_ReturnsOkResult_WithGraphic()
        {
            // Arrange
            var graphic = new ProjectGraphic { ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };

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
            var graphic = new ProjectGraphic { ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };
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
            var graphic = new ProjectGraphic { Id = new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(new Guid("111223344-5566-7788-99PG-BBCCDDEEFF00234567"), graphic);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ProjectGraphic>(okResult.Value);
            Assert.Equal(new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), returnValue.Id);
        }

        [Fact]
        public async Task UpdateProjectGraphicById_ReturnsNotFound_WhenGraphicNotFound()
        {
            // Arrange
            var graphic = new ProjectGraphic { Id = new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), ProjectId = new Guid("11223344-5566-7788-99PP-BBCCDDEEFF00"), JSONData = "{}" };

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(new Guid("11223344-5566-7788-99NF-BBCCDDEEFF00"), graphic);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProjectGraphicByIdAsync_ReturnsBadRequest_OnException()
        {
            // Arrange
            var graphic = new ProjectGraphic { Id = new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), ProjectId = new Guid("123411223344-5566-7788-99PP-BBCCDDEEFF00567"), JSONData = "{}" };
            _context.Database.EnsureDeleted(); // Simulate an exception by deleting the database

            // Act
            var result = await _controller.UpdateProjectGraphicByIdAsync(new Guid("11223344-5566-7788-99PG-BBCCDDEEFF00"), graphic);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        
    }
}