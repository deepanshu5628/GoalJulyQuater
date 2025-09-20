using FluentValidation;
using GoalQuater1.Data;
using GoalQuater1.Models;
using GoalQuater1.Models.Dto_s;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoalQuater1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IValidator<CustomerDto> _validator;

        public CustomersController(OrderDbContext dbContext, IValidator<CustomerDto> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var query = _dbContext.Customers.AsQueryable();
            var customers = await query.ToListAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            return customer != null ? Ok(customer) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Region = dto.Region,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto dto)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null) return NotFound();

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.Region = dto.Region;
            await _dbContext.SaveChangesAsync();
            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
