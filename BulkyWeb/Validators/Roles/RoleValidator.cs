using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BulkyWeb.Validators.Roles
{
    public class RoleValidator : AbstractValidator<IdentityRole>
    {
        public RoleValidator()
        {

            RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Role name is required.")
                    .MaximumLength(256).WithMessage("Role name cannot exceed 256 characters.");

            RuleFor(x => x.NormalizedName)
                                    .MaximumLength(256).WithMessage("Description cannot exceed 256s characters.");

        }
    }
}
