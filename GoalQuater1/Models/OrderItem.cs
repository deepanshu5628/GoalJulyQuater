using Microsoft.EntityFrameworkCore;

namespace GoalQuater1.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public string Sku { get; set; } // e.g., "PROD-001"
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        public Order Order { get; set; } // Navigation
    }
}
