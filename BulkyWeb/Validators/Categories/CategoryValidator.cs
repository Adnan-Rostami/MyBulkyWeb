using BulkyWeb.Models;
using FluentValidation;
namespace BulkyWeb.Validators.Categories
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {

            RuleFor(x => x.CategoryName)
                    .NotEmpty().WithMessage("Category name is required.")
                    .MaximumLength(15).WithMessage("Category name cannot exceed 15 characters.");

            RuleFor(x => x.Description)
                                    .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        }
    }
}

