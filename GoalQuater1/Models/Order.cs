using Microsoft.EntityFrameworkCore;

namespace GoalQuater1.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } // e.g., "Pending", "Shipped"
        public string PaymentMethod { get; set; } // e.g., "CreditCard"

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public Customer Customer { get; set; } // Navigation
        public List<OrderItem> OrderItems { get; set; }
    }
}
