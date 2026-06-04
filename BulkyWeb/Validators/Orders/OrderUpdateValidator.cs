using BulkyWeb.Models.DTO.Order;
using FluentValidation;

namespace BulkyWeb.Validators.Orders
{
    public class OrderUpdateValidator : AbstractValidator<OrderUpdateDTO>
    {
        public OrderUpdateValidator()
        {


            {
                RuleFor(x => x.Freight)
                    .GreaterThanOrEqualTo(0)
                    .When(x => x.Freight.HasValue);

                //RuleFor(x => x.RequiredDate)
                //    .GreaterThanOrEqualTo(x => x.OrderDate)
                //    .WithMessage("تاریخ تحویل نمی‌تواند قبل از تاریخ سفارش باشد");


                RuleFor(x => x)
    .Must(x => !x.RequiredDate.HasValue || x.RequiredDate >= x.OrderDate)
    .WithMessage("RequiredDate cannot be before OrderDate");


                RuleFor(x => x.ShipName)
                    .MaximumLength(40);

                RuleFor(x => x.ShipAddress)
                    .MaximumLength(60);

                RuleFor(x => x.ShipCity)
                    .MaximumLength(15);

                RuleFor(x => x.ShipPostalCode)
                    .MaximumLength(10);

                RuleFor(x => x.ShipCountry)
                    .MaximumLength(15);



                RuleFor(x => x.Items)
         .NotNull().WithMessage("Items cannot be null.")
         .Must(items => items != null && items.Any())
         .WithMessage("Items cannot be empty.")
         .Must(items => items != null && items.GroupBy(i => i.ProductID).All(g => g.Count() == 1))
         .WithMessage("Duplicate products are not allowed in order items.");


                When(x => x.Items != null, () =>
                {
                    RuleForEach(x => x.Items!)
                        .SetValidator(new OrderItemValidator());
                });
            }
        }
    }
}

