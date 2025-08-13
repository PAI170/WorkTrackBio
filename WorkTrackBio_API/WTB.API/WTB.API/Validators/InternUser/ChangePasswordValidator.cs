using FluentValidation;
using WTB.API.Models.DTOs.InternUser;
using System.Text.RegularExpressions;

namespace WTB.API.Validators.InternUser
{
    /// <summary>
    /// Validador para cambio de contraseña de usuarios administrativos
    /// </summary>
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            // Validación del ID de usuario
            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("El ID del usuario debe ser mayor que cero");

            // Validación de contraseña actual
            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("La contraseña actual es obligatoria")
                .MinimumLength(1)
                .WithMessage("Debe proporcionar la contraseña actual");

            // Validación de nueva contraseña
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("La nueva contraseña es obligatoria")
                .MinimumLength(8)
                .WithMessage("La nueva contraseña debe tener al menos 8 caracteres")
                .MaximumLength(100)
                .WithMessage("La nueva contraseña no puede exceder 100 caracteres")
                .Must(BeStrongPassword)
                .WithMessage("La nueva contraseña debe contener al menos: 1 mayúscula, 1 minúscula, 1 número y 1 carácter especial")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("La nueva contraseña debe ser diferente a la actual");

            // Validación de confirmación de nueva contraseña
            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Debe confirmar la nueva contraseña")
                .Equal(x => x.NewPassword)
                .WithMessage("Las nuevas contraseñas no coinciden");

            // Validación de seguridad adicional
            RuleFor(x => x)
                .Must(x => !BePasswordTooSimilarToUser(x.NewPassword, x.UserId))
                .WithMessage("La nueva contraseña no debe ser similar a información personal del usuario")
                .WithName("NewPassword");

            // TODO: Validaciones que requieren acceso a datos:
            // - El usuario debe existir y estar activo
            // - La contraseña actual debe ser correcta
            // - La nueva contraseña no debe ser una de las últimas 5 utilizadas
            // Estas validaciones se implementarán en el servicio
        }

        /// <summary>
        /// Valida que la contraseña sea suficientemente fuerte
        /// </summary>
        private bool BeStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Al menos una mayúscula
            if (!Regex.IsMatch(password, @"[A-Z]"))
                return false;

            // Al menos una minúscula
            if (!Regex.IsMatch(password, @"[a-z]"))
                return false;

            // Al menos un número
            if (!Regex.IsMatch(password, @"\d"))
                return false;

            // Al menos un carácter especial
            if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]"))
                return false;

            // No debe contener secuencias obvias
            var commonPatterns = new[]
            {
                "123456", "abcdef", "qwerty", "password", "admin", "123123"
            };

            var lowerPassword = password.ToLowerInvariant();
            if (commonPatterns.Any(pattern => lowerPassword.Contains(pattern)))
                return false;

            return true;
        }

        /// <summary>
        /// Valida que la contraseña no sea muy similar a la información del usuario
        /// </summary>
        private bool BePasswordTooSimilarToUser(string password, int userId)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // TODO: Implementar validación real consultando información del usuario
            // Por ahora, validaciones básicas
            var lowerPassword = password.ToLowerInvariant();
            
            // No debe contener el ID del usuario
            if (lowerPassword.Contains(userId.ToString()))
                return true;

            // No debe ser solo números consecutivos
            if (Regex.IsMatch(password, @"^(\d)\1+$")) // 111111, 222222, etc.
                return true;

            return false;
        }
    }
}




