namespace GoalQuater1.Models.Dto_s
{
    public record OrderDto(int CustomerId, DateTime OrderDate, string Status, string PaymentMethod, decimal TotalAmount);
}
