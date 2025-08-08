using FluentValidation;
using WTB.API.Models.DTOs.InternUser;
using System.Text.RegularExpressions;

namespace WTB.API.Validators.InternUser
{
    /// <summary>
    /// Validador para la creación de usuarios administrativos
    /// </summary>
    public class CreateInternUserValidator : AbstractValidator<CreateInternUserDto>
    {
        public CreateInternUserValidator()
        {
            // Validación de email
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("El email es obligatorio")
                .EmailAddress()
                .WithMessage("El formato del email no es válido")
                .MaximumLength(100)
                .WithMessage("El email no puede exceder 100 caracteres")
                .Must(BeValidEmailDomain)
                .WithMessage("El dominio del email no está permitido");

            // Validación de nombre
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("El nombre es obligatorio")
                .Length(2, 50)
                .WithMessage("El nombre debe tener entre 2 y 50 caracteres")
                .Must(BeValidName)
                .WithMessage("El nombre solo puede contener letras, espacios y tildes");

            // Validación de apellido
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("El apellido es obligatorio")
                .Length(2, 100)
                .WithMessage("El apellido debe tener entre 2 y 100 caracteres")
                .Must(BeValidName)
                .WithMessage("El apellido solo puede contener letras, espacios y tildes");

            // Validación de contraseña
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("La contraseña es obligatoria")
                .MinimumLength(8)
                .WithMessage("La contraseña debe tener al menos 8 caracteres")
                .MaximumLength(100)
                .WithMessage("La contraseña no puede exceder 100 caracteres")
                .Must(BeStrongPassword)
                .WithMessage("La contraseña debe contener al menos: 1 mayúscula, 1 minúscula, 1 número y 1 carácter especial");

            // Validación de confirmación de contraseña
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Debe confirmar la contraseña")
                .Equal(x => x.Password)
                .WithMessage("Las contraseñas no coinciden");

            // Validación de rol
            RuleFor(x => x.RolId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un rol válido")
                .Must(BeValidRole)
                .WithMessage("El rol seleccionado no existe o no está activo");

            // TODO: Validaciones que requieren acceso a datos:
            // - El email no debe estar registrado ya
            // - El rol debe existir y estar activo
            // Estas validaciones se implementarán en el servicio
        }

        /// <summary>
        /// Valida que el dominio del email esté permitido
        /// </summary>
        private bool BeValidEmailDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Lista de dominios permitidos (puedes expandir esta lista)
            var allowedDomains = new[]
            {
                "gmail.com", "outlook.com", "hotmail.com", "yahoo.com",
                "empresa.com", "worktrackbio.com" // Dominios de la empresa
            };

            try
            {
                var domain = email.Split('@')[1].ToLowerInvariant();
                return allowedDomains.Contains(domain) || domain.EndsWith(".edu") || domain.EndsWith(".gov");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida que el nombre contenga solo caracteres válidos
        /// </summary>
        private bool BeValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Permitir letras (incluidas con tildes), espacios y guiones
            var namePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-'\.]+$";
            return Regex.IsMatch(name, namePattern);
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
                "123456", "abcdef", "qwerty", "password", "admin"
            };

            var lowerPassword = password.ToLowerInvariant();
            if (commonPatterns.Any(pattern => lowerPassword.Contains(pattern)))
                return false;

            return true;
        }

        /// <summary>
        /// Valida que el rol sea válido (mock - se implementará con acceso a datos)
        /// </summary>
        private bool BeValidRole(int rolId)
        {
            // TODO: Implementar validación real con acceso a base de datos
            // return await _context.Roles.AnyAsync(r => r.Id == rolId && r.IsActive);
            
            // Mock temporal - roles válidos típicos
            var validRoles = new[] { 1, 2, 3 }; // Admin, Manager, Viewer
            return validRoles.Contains(rolId);
        }
    }
}
