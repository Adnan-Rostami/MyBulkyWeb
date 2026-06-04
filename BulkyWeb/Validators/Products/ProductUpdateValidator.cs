using BulkyWeb.Models.DTO.Product;
using FluentValidation;

namespace BulkyWeb.Validators.Products
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDTO>
    {
        public ProductUpdateValidator()
        {
            When(x => x.ProductName != null, () =>
            {
                RuleFor(x => x.ProductName!)
                    .NotEmpty()
                    .MaximumLength(40);
            });

            When(x => x.CategoryID.HasValue, () =>
            {
                RuleFor(x => x.CategoryID!.Value)
                    .GreaterThan(0);
            });

            When(x => x.UnitPrice.HasValue, () =>
            {
                RuleFor(x => x.UnitPrice!.Value)
                    .GreaterThanOrEqualTo(0);
            });

            When(x => x.QuantityPerUnit != null, () =>
            {
                RuleFor(x => x.QuantityPerUnit!)
                    .MaximumLength(20);
            });

            When(x => x.UnitsInStock.HasValue, () =>
            {
                RuleFor(x => x.UnitsInStock!.Value)
                    .GreaterThanOrEqualTo((short)0);
            });

        }
    }
}
