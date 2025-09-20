using FluentValidation;
using GoalQuater1.Models.Dto_s;

namespace GoalQuater1.Services
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Region).NotEmpty().Must(r => new[] { "Rajasthan", "Haryana", "Delhi", "Punjab", "UP" }.Contains(r));
        }
    }

    public class OrderValidator : AbstractValidator<OrderDto>
    {
        public OrderValidator()
        {
            RuleFor(x => x.CustomerId).GreaterThan(0);
            RuleFor(x => x.OrderDate).NotEmpty();
            RuleFor(x => x.Status).NotEmpty().Must(s => new[] { "Pending", "Shipped", "Delivered", "Cancelled" }.Contains(s));
            RuleFor(x => x.PaymentMethod).NotEmpty().Must(p => new[] { "CreditCard", "PayPal", "BankTransfer" }.Contains(p));
            RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
        }
    }

    public class OrderItemValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.Sku).NotEmpty().Matches(@"^PROD-\d{3}$");
            RuleFor(x => x.Quantity).InclusiveBetween(1, 100);
            RuleFor(x => x.UnitPrice).GreaterThan(0);
        }
    }
}
