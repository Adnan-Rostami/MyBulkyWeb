using BulkyWeb.Models.DTO.Order;
using FluentValidation;

namespace BulkyWeb.Validators.Orders
{
    public class OrderItemValidator : AbstractValidator<OrderDetailDTO>
    {
        public OrderItemValidator()
        {
            RuleFor(x => x.ProductID)
               .GreaterThan(0);




            //RuleFor(x => x.UnitPrice)
            //    .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Quantity)
                .GreaterThan((short)0);

            //RuleFor(x => x.Discount)
            //    .InclusiveBetween(0, 1);
        }
    }
}
