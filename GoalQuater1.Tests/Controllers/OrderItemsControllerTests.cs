using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using GoalQuater1.Controllers;
using GoalQuater1.Data;
using GoalQuater1.Models;
using GoalQuater1.Models.Dto_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GoalQuater1.Tests.Controllers
{
    public class OrderItemsControllerTests
    {
        private readonly OrderItemsController _controller;
        private readonly OrderDbContext _dbContext;
        private readonly Mock<IValidator<OrderItemDto>> _validatorMock;

        public OrderItemsControllerTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_OrderItems")
                .Options;
            _dbContext = new OrderDbContext(options);
            _validatorMock = new Mock<IValidator<OrderItemDto>>();
            _controller = new OrderItemsController(_dbContext, _validatorMock.Object);
        }

        [Fact]
        public async Task GetOrderItems_ReturnsOkResult()
        {
            _dbContext.OrderItems.Add(new OrderItem { OrderId = 1, Sku = "PROD-001", Quantity = 2, UnitPrice = 10 });
            await _dbContext.SaveChangesAsync();
            var result = await _controller.GetOrderItems(null);
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetOrderItem_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.GetOrderItem(999);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateOrderItem_ReturnsBadRequest_WhenInvalid()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<OrderItemDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure("Sku", "Required") }));
            var dto = new OrderItemDto(0, "", 0, 0);
            var result = await _controller.CreateOrderItem(dto);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateOrderItem_ReturnsCreatedAtAction_WhenValid()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<OrderItemDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var dto = new OrderItemDto(1, "PROD-001", 2, 10);
            var result = await _controller.CreateOrderItem(dto);
            result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}
