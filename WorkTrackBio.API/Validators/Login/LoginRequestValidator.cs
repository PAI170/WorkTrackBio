using FluentValidation;
using WorkTrackBio.API.DataTransferObjects.Auth;

namespace WorkTrackBio.API.Validators.Auth
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.WorkEmail)
                .NotEmpty()
                .WithMessage("El correo es requerido.")
                .EmailAddress()
                .WithMessage("El formato del correo no es válido.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("La contraseña es requerida.")
                .MinimumLength(8)
                .WithMessage("La contraseña debe tener al menos 8 caracteres.");
        }
    }
}