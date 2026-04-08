using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.AppUser;

namespace WorkTrackBio.API.Validators.AppUser
{
    public class AppUserUpdateValidator : AbstractValidator<AppUserUpdateDto>
    {
        public AppUserUpdateValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("El rol es obligatorio.");

            RuleFor(x => x.WorkEmail)
                .NotEmpty().WithMessage("El correo de trabajo es obligatorio.")
                .EmailAddress().WithMessage("El correo de trabajo no tiene un formato válido.")
                .MaximumLength(100).WithMessage("El correo no puede superar 100 caracteres.");
        }
    }
}