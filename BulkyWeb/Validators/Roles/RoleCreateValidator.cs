using BulkyWeb.Models.RoleModels;
using FluentValidation;

namespace BulkyWeb.Validators.Roles
{
    public class RoleCreateValidator : AbstractValidator<RoleCreateDTO>
    {
        public RoleCreateValidator()
        {

            RuleFor(x => x.Name)
                      .NotEmpty().WithMessage("Role name is required.")
                      .MaximumLength(256).WithMessage("Role name cannot exceed 256 characters.");



        }
    }
}
