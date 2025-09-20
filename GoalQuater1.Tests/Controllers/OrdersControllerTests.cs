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
    public class OrdersControllerTests
    {
        private readonly OrdersController _controller;
        private readonly OrderDbContext _dbContext;
        private readonly Mock<IValidator<OrderDto>> _validatorMock;

        public OrdersControllerTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Orders")
                .Options;
            _dbContext = new OrderDbContext(options);
            _validatorMock = new Mock<IValidator<OrderDto>>();
            _controller = new OrdersController(_dbContext, _validatorMock.Object);
        }

        [Fact]
        public async Task GetOrders_ReturnsOkResult()
        {
            _dbContext.Orders.Add(new Order { CustomerId = 1, OrderDate = System.DateTime.UtcNow, Status = "Pending", PaymentMethod = "CreditCard", TotalAmount = 100 });
            await _dbContext.SaveChangesAsync();
            var result = await _controller.GetOrders();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetOrder_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.GetOrder(999);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenInvalid()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<OrderDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure("CustomerId", "Required") }));
            var dto = new OrderDto(0, System.DateTime.UtcNow, "", "", 0);
            var result = await _controller.CreateOrder(dto);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedAtAction_WhenValid()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<OrderDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var dto = new OrderDto(1, System.DateTime.UtcNow, "Pending", "CreditCard", 100);
            var result = await _controller.CreateOrder(dto);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task UpdateOrder_ReturnsNotFound_WhenNotExists()
        {
            var dto = new OrderDto(1, System.DateTime.UtcNow, "Pending", "CreditCard", 100);
            var result = await _controller.UpdateOrder(999, dto);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteOrder_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.DeleteOrder(999);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
