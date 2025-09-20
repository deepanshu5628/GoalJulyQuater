using GoalQuater1.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalQuater1.Data
{
    public class OrderDbContext :  DbContext
    {
        public OrderDbContext(DbContextOptions options) :base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
