using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Validators.Role
{
    public class RoleCreateValidator : AbstractValidator<RoleCreateDto>
    {
        public RoleCreateValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede superar 50 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede superar 255 caracteres.")
                .When(x => x.Description != null);
        }
    }
}