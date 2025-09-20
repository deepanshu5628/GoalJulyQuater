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
    public class OrderItemsController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IValidator<OrderItemDto> _validator;

        public OrderItemsController(OrderDbContext dbContext, IValidator<OrderItemDto> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderItems([FromQuery] int? orderId)
        {
            var query = _dbContext.OrderItems.AsQueryable();
            if (orderId.HasValue)
                query = query.Where(oi => oi.OrderId == orderId.Value);
            var items = await query.ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderItem(int id)
        {
            var item = await _dbContext.OrderItems.FindAsync(id);
            return item != null ? Ok(item) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderItem([FromBody] OrderItemDto dto)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var item = new OrderItem
            {
                OrderId = dto.OrderId,
                Sku = dto.Sku,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };
            _dbContext.OrderItems.Add(item);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderItem), new { id = item.OrderItemId }, item);
        }
    }
}
