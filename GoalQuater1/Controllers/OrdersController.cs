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
    public class OrdersController : ControllerBase
    {

        private readonly OrderDbContext _dbContext;
        private readonly IValidator<OrderDto> _validator;

        public OrdersController(OrderDbContext dbContext, IValidator<OrderDto> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var query = _dbContext.Orders.Include(o => o.Customer).AsQueryable();
           
            var orders = await query.ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return order != null ? Ok(order) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = dto.OrderDate,
                Status = dto.Status,
                PaymentMethod = dto.PaymentMethod,
                TotalAmount = dto.TotalAmount
            };
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderDto dto)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            order.CustomerId = dto.CustomerId;
            order.OrderDate = dto.OrderDate;
            order.Status = dto.Status;
            order.PaymentMethod = dto.PaymentMethod;
            order.TotalAmount = dto.TotalAmount;
            await _dbContext.SaveChangesAsync();
            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null) return NotFound();
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

