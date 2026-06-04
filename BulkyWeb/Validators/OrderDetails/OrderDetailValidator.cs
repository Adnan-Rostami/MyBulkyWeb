using BulkyWeb.Models;
using FluentValidation;

namespace BulkyWeb.Validators.OrderDetails
{
    public class OrderDetailValidator : AbstractValidator<OrderDetail>
    {
        public OrderDetailValidator()
        {
            RuleFor(x => x.ProductID)
            .GreaterThan(0);

            RuleFor(x => x.OrderID)
            .GreaterThan(0);

            RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Quantity)
            .GreaterThan((short)0);

            RuleFor(x => x.Discount)
            .InclusiveBetween(0, 1);
        }

    }
}
