using BulkyWeb.Models.DTO.Product;
using FluentValidation;

namespace BulkyWeb.Validators.Products
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDTO>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("نام محصول اجباری است")
            .MaximumLength(40);

            RuleFor(x => x.CategoryID)
                .GreaterThan(0);

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.QuantityPerUnit)
                .MaximumLength(20);

            RuleFor(x => x.UnitsInStock)
                .GreaterThanOrEqualTo((short)0);



        }


    }
}
