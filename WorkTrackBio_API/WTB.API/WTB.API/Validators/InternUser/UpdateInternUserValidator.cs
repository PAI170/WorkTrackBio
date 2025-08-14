using FluentValidation;
using WTB.API.Models.DTOs.InternUser;
using System.Text.RegularExpressions;

namespace WTB.API.Validators.InternUser
{
    /// <summary>
    /// Validador para la actualización de usuarios administrativos
    /// </summary>
    public class UpdateInternUserValidator : AbstractValidator<UpdateInternUserDto>
    {
        public UpdateInternUserValidator()
        {
            // Validación del ID
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("El ID del usuario debe ser mayor que cero");

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

            // Validación de rol
            RuleFor(x => x.RolId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un rol válido")
                .Must(BeValidRole)
                .WithMessage("El rol seleccionado no existe o no está activo");

            // Validación de estado
            RuleFor(x => x.StateId)
                .GreaterThan(0)
                .WithMessage("Debe seleccionar un estado válido")
                .Must(BeValidState)
                .WithMessage("El estado seleccionado no existe");

            // TODO: Validaciones que requieren acceso a datos:
            // - El usuario debe existir
            // - El email no debe estar registrado por otro usuario
            // - No se puede desactivar el último administrador
            // Estas validaciones se implementarán en el servicio
        }

        /// <summary>
        /// Valida que el dominio del email esté permitido
        /// </summary>
        private bool BeValidEmailDomain(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var allowedDomains = new[]
            {
                "gmail.com", "outlook.com", "hotmail.com", "yahoo.com",
                "empresa.com", "worktrackbio.com"
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

            var namePattern = @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s\-'\.]+$";
            return Regex.IsMatch(name, namePattern);
        }

        /// <summary>
        /// Valida que el rol sea válido
        /// </summary>
        private bool BeValidRole(int rolId)
        {
            // TODO: Implementar validación real
            var validRoles = new[] { 1, 2, 3 }; // Admin, Manager, Viewer
            return validRoles.Contains(rolId);
        }

        /// <summary>
        /// Valida que el estado sea válido
        /// </summary>
        private bool BeValidState(int stateId)
        {
            // TODO: Implementar validación real
            var validStates = new[] { 1, 2 }; // Activo, Inactivo
            return validStates.Contains(stateId);
        }
    }
}






