using Bogus;
using GoalQuater1.Data;
using GoalQuater1.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GoalQuater1.Services
{
    public class DataSeeder
    {
        private readonly OrderDbContext _dbContext;
        public DataSeeder(OrderDbContext dbcontext)
        {
            this._dbContext = dbcontext;
        }

        public async Task SeedAsync()
        {
            // Check if data already exists to avoid duplicates
            if (await _dbContext.Customers.AnyAsync()) return;

            // Define regions and other enums
            var regions = new[] { "Rajasthan", "Haryana", "Delhi", "Punjab", "UP" };
            var statuses = new[] { "Pending", "Shipped", "Delivered", "Cancelled" };
            var paymentMethods = new[] { "CreditCard", "PayPal", "BankTransfer" };

            // Faker for Customers
            var customerFaker = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .RuleFor(c => c.Region, f => f.PickRandom(regions))
                .RuleFor(c => c.CreatedAt, f => f.Date.Past(2)); // Last 2 years

            var customers = customerFaker.Generate(50); // ~50 customers
            await _dbContext.Customers.AddRangeAsync(customers);
            await _dbContext.SaveChangesAsync(); // Save to get IDs

            // Faker for Orders (linked to customers)
            var orderFaker = new Faker<Order>()
                .RuleFor(o => o.CustomerId, f => f.PickRandom(customers).CustomerId)
                .RuleFor(o => o.OrderDate, f => f.Date.Past(1)) // Last year, skewed recent
                .RuleFor(o => o.Status, f => f.PickRandom (statuses)) // Mostly Delivered
                .RuleFor(o => o.PaymentMethod, f => f.PickRandom(paymentMethods))
                .RuleFor(o => o.TotalAmount, 0m); // Placeholder, calculate later

            var orders = new List<Order>();
            foreach (var customer in customers)
            {
                var numOrders = new Random().Next(5, 15); // Avg 10 per customer -> ~500 total
                orders.AddRange(orderFaker.Generate(numOrders));
            }
            await _dbContext.Orders.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync(); // Save to get OrderIDs

            // Faker for OrderItems
            var orderItemFaker = new Faker<OrderItem>()
                .RuleFor(oi => oi.OrderId, f => f.PickRandom(orders).OrderId)
                .RuleFor(oi => oi.Sku, f => $"PROD-{f.Random.Number(1, 1000):D3}")
                .RuleFor(oi => oi.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(oi => oi.UnitPrice, f => f.Random.Decimal(10, 200));

            var orderItems = new List<OrderItem>();
            foreach (var order in orders)
            {
                var numItems = new Random().Next(2, 5); // Avg 3 per order -> ~1500 total
                var items = orderItemFaker.Generate(numItems);
                orderItems.AddRange(items);

                // Calculate TotalAmount for the order
                order.TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice);
            }

            await _dbContext.OrderItems.AddRangeAsync(orderItems);
            await _dbContext.SaveChangesAsync(); // Final save

            // Update orders with calculated totals
            _dbContext.Orders.UpdateRange(orders);
            await _dbContext.SaveChangesAsync();
        }
    }
}
