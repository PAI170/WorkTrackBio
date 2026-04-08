using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.AppUser;

namespace WorkTrackBio.API.Validators.AppUser
{
    public class AppUserCreateValidator : AbstractValidator<AppUserCreateDto>
    {
        public AppUserCreateValidator()
        {
            RuleFor(x => x.EmployeeInfoId)
                .GreaterThan(0).WithMessage("El empleado es obligatorio.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("El rol es obligatorio.");

            RuleFor(x => x.WorkEmail)
                .NotEmpty().WithMessage("El correo de trabajo es obligatorio.")
                .EmailAddress().WithMessage("El correo de trabajo no tiene un formato válido.")
                .MaximumLength(100).WithMessage("El correo no puede superar 100 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
                .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
                .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
                .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.")
                .Matches("[^a-zA-Z0-9]").WithMessage("La contraseña debe contener al menos un carácter especial.");
        }
    }
}