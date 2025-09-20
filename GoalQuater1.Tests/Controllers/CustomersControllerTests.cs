using System.Collections.Generic;
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
    public class CustomersControllerTests
    {
        private readonly CustomersController _controller;
        private readonly OrderDbContext _dbContext;
        private readonly Mock<IValidator<CustomerDto>> _validatorMock;

        public CustomersControllerTests()
        {
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Customers")
                .Options;
            _dbContext = new OrderDbContext(options);
            _validatorMock = new Mock<IValidator<CustomerDto>>();
            _controller = new CustomersController(_dbContext, _validatorMock.Object);
        }

        [Fact]
        public async Task GetCustomers_ReturnsOkResult()
        {
            // Arrange
            _dbContext.Customers.Add(new Customer { Name = "Test", Email = "test@email.com", Region = "Delhi" });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetCustomers();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetCustomer_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.GetCustomer(999);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateCustomer_ReturnsBadRequest_WhenInvalid()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CustomerDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[] { new FluentValidation.Results.ValidationFailure("Name", "Required") }));
            var dto = new CustomerDto("", "", "");
            var result = await _controller.CreateCustomer(dto);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task CreateCustomer_ReturnsCreatedAtAction_WhenValid()
        {
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<CustomerDto>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            var dto = new CustomerDto("Test", "test@email.com", "Delhi");
            var result = await _controller.CreateCustomer(dto);
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task UpdateCustomer_ReturnsNotFound_WhenNotExists()
        {
            var dto = new CustomerDto("Test", "test@email.com", "Delhi");
            var result = await _controller.UpdateCustomer(999, dto);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteCustomer_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.DeleteCustomer(999);
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
