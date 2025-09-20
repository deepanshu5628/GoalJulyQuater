using Azure.Core;
using CsvHelper;
using GoalQuater1.Data;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GoalQuater1.Controllers
{
    public class ExportController : ControllerBase
    {
        private readonly OrderDbContext _dbContext;
        private readonly IConfiguration _config;

        public ExportController(OrderDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        [HttpGet("orders")]
        [Produces("text/csv")]
        public async Task<IActionResult> ExportOrders([FromQuery] string? since)
        {
            try
            {
                // API key check
                if (!Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || apiKey != _config["ApiKey"])
                    return Unauthorized();

                // Parse since date or get all
                DateTime? sinceDate = null;
                if (since != null)
                {
                    try
                    {
                        sinceDate = DateTime.ParseExact(since, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        return BadRequest("Invalid date format. Use YYYY-MM-DD (e.g., 2025-01-01).");
                    }
                }

                // Query orders with related data
                var orders = await _dbContext.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                    .Where(o => !sinceDate.HasValue || o.OrderDate >= sinceDate.Value)
                    .ToListAsync();

                // Flatten to CSV-friendly format
                var records = orders.SelectMany(o => o.OrderItems.Select(oi => new
                {
                    o.OrderId,
                    CustomerName = o.Customer?.Name ?? "Unknown", // Handle null Customer
                    o.OrderDate,
                    o.Status,
                    o.PaymentMethod,
                    o.TotalAmount,
                    oi.OrderItemId,
                    oi.Sku,
                    oi.Quantity,
                    oi.UnitPrice
                }));

                // Write CSV to stream
                var memoryStream = new MemoryStream(); // No 'using' to keep stream open
                using (var writer = new StreamWriter(memoryStream, leaveOpen: true)) // Keep stream open
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    await csv.WriteRecordsAsync(records);
                    await writer.FlushAsync();
                }
                memoryStream.Position = 0;

                return File(memoryStream, "text/csv", "orders_export.csv");
            }
            catch (Exception ex)
            {
                // Log error (use Serilog or console for POC)
                Console.WriteLine($"Error in ExportOrders: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
    }
}
