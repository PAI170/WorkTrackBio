using FluentValidation;
using WTB.API.Models.DTOs.Auth;

namespace WTB.API.Validators.Auth
{
    /// <summary>
    /// Validador para inicio de sesión
    /// </summary>
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            // Validación de email
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("El email es obligatorio")
                .EmailAddress()
                .WithMessage("El formato del email no es válido")
                .MaximumLength(100)
                .WithMessage("El email no puede exceder 100 caracteres");

            // Validación de contraseña
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("La contraseña es obligatoria")
                .MinimumLength(1)
                .WithMessage("Debe proporcionar una contraseña")
                .MaximumLength(100)
                .WithMessage("La contraseña no puede exceder 100 caracteres");

            // TODO: Validaciones adicionales que requieren acceso a datos:
            // - Rate limiting (máximo intentos por IP/usuario)
            // - Account lockout después de varios intentos fallidos
            // - Verificación de usuario activo
            // Estas validaciones se implementarán en el servicio de autenticación
        }
    }
}






