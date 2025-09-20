using System.Threading.Tasks;
using FluentAssertions;
using GoalQuater1.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using GoalQuater1.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GoalQuater1.Tests.Controllers
{
    public class ExportControllerTests
    {
        [Fact]
        public async Task ExportController_ExportOrdersEndpoint_ReturnsFileResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Export")
                .Options;
            var dbContext = new OrderDbContext(options);
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["ApiKey"]).Returns("test-key");
            var controller = new ExportController(dbContext, configMock.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Headers["X-Api-Key"] = "test-key";
            // Act
            var result = await controller.ExportOrders(null);
            // Assert
            result.Should().BeAssignableTo<FileResult>();
        }
    }
}
