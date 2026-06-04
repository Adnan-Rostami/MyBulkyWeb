using BulkyWeb.Models.DTO.Order;
using FluentValidation;

namespace BulkyWeb.Validators.Orders
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateDTO>
    {
        public OrderCreateValidator()
        {
            //     RuleFor(x => x.userId).NotEmpty();
            //.Length(5).When(x => x.userId != null);

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

            RuleFor(x => x.Freight)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Freight.HasValue);
            //        RuleFor(x => x.Items)
            //.Must(items => items.Select(i => i.ProductID).Distinct().Count() == items.Count)
            //.WithMessage("Duplicate products are not allowed in order items.");

            RuleFor(x => x.Items)
    .NotNull().WithMessage("Items cannot be null.")
    .Must(items => items != null && items.Any())
    .WithMessage("Items cannot be empty.")
    .Must(items => items!.GroupBy(i => i.ProductID).All(g => g.Count() == 1))
    .WithMessage("Duplicate products are not allowed in order items.");




            //        RuleForEach(x => x.Items)
            //.SetValidator(new OrderItemValidator());
            //    }



            When(x => x.Items != null, () =>
            {
                RuleForEach(x => x.Items!)
                    .SetValidator(new OrderItemValidator());
            });






            //        RuleFor(x => x)
            //.Must(x =>
            //    !x.RequiredDate.HasValue ||
            //    !x.OrderDate.HasValue ||
            //    x.RequiredDate >= x.OrderDate)
            //.WithMessage("RequiredDate cannot be before OrderDate");

        }
    }
}
