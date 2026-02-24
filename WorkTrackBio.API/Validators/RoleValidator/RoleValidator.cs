using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Role;
using WorkTrackBio.API.Repositories.RoleRepository;

namespace WorkTrackBio.API.Validators.RoleValidator
{
    public class RoleValidator : IRoleValidator
    {
        private readonly IValidator<CreateRoleDataTransferObject> _createValidator;
        private readonly IValidator<UpdateRoleDataTransferObject> _updateValidator;
        private readonly IRoleRepository _roleRepository;

        public RoleValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _createValidator = new CreateRoleValidator();
            _updateValidator = new UpdateRoleValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateRoleDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del rol no pueden estar vacíos"));
                return result;
            }

            // Validaciones básicas de formato
            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            // Validación de unicidad del nombre
            var nameUniqueValidation = await ValidateRoleNameUniqueAsync(createDto.RoleName);
            if (!nameUniqueValidation.IsValid)
            {
                // Combinar errores
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in nameUniqueValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            return basicValidation;
        }
        public async Task<ValidationResult> ValidateUpdateAsync(UpdateRoleDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del rol no pueden estar vacíos"));
                return result;
            }

            // Validaciones básicas de formato
            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            // Validación de existencia del ID
            var existsValidation = await ValidateRoleExistsAsync(updateDto.Id);
            if (!existsValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in existsValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            // Validación de unicidad del nombre (excluyendo el ID actual)
            // Solo validar si se proporcionó un nombre
            if (!string.IsNullOrWhiteSpace(updateDto.RoleName))
            {
                var nameUniqueValidation = await ValidateRoleNameUniqueAsync(updateDto.RoleName, updateDto.Id);
                if (!nameUniqueValidation.IsValid)
                {
                    var combinedResult = new ValidationResult();
                    foreach (var error in basicValidation.Errors)
                        combinedResult.Errors.Add(error);
                    foreach (var error in nameUniqueValidation.Errors)
                        combinedResult.Errors.Add(error);
                    return combinedResult;
                }
            }

            return basicValidation;
        }

        public ValidationResult ValidateId(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
            }

            return result;
        }

        public ValidationResult ValidateRoleName(string roleName)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(roleName))
            {
                result.Errors.Add(new ValidationFailure("RoleName", "El nombre del rol no puede estar vacío"));
            }
            else if (roleName.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("RoleName", "El nombre del rol no puede exceder 50 caracteres"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateRoleExistsAsync(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
                return result;
            }

            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    result.Errors.Add(new ValidationFailure("Id", $"No existe un rol con ID {id}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("Id", $"Error al validar la existencia del rol: {ex.Message}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateRoleNameUniqueAsync(string roleName, int? excludeId = null)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(roleName))
            {
                result.Errors.Add(new ValidationFailure("RoleName", "El nombre del rol no puede estar vacío"));
                return result;
            }

            try
            {
                var exists = await _roleRepository.ExistsByNameAsync(roleName);
                if (exists)
                {
                    // Si estamos actualizando, verificar si el nombre pertenece al mismo rol
                    if (excludeId.HasValue)
                    {
                        var allRoles = await _roleRepository.GetAllAsync();
                        var conflictingRole = allRoles.FirstOrDefault(r => 
                            r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase) && 
                            r.Id != excludeId.Value);
                        
                        if (conflictingRole != null)
                        {
                            result.Errors.Add(new ValidationFailure("RoleName", 
                                $"Ya existe otro rol con el nombre '{roleName}'"));
                        }
                    }
                    else
                    {
                        // Para creación, cualquier duplicado es inválido
                        result.Errors.Add(new ValidationFailure("RoleName", 
                            $"Ya existe un rol con el nombre '{roleName}'"));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("RoleName", 
                    $"Error al validar la unicidad del nombre: {ex.Message}"));
            }

            return result;
        }
    }

    public class CreateRoleValidator : AbstractValidator<CreateRoleDataTransferObject>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("El nombre del rol es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del rol no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El nombre del rol no puede estar vacío o contener solo espacios (restricción de BD)");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede exceder 255 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }

    public class UpdateRoleValidator : AbstractValidator<UpdateRoleDataTransferObject>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor que 0")
                .LessThan(int.MaxValue).WithMessage("El ID excede el valor máximo permitido");

            // RoleName es opcional en updates - solo validar si se proporciona
            RuleFor(x => x.RoleName)
                .MaximumLength(50).WithMessage("El nombre del rol no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un nombre, no puede estar vacío o contener solo espacios")
                .When(x => x.RoleName != null);

            // Description es opcional en updates - solo validar si se proporciona
            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede exceder 255 caracteres (restricción de BD)")
                .When(x => x.Description != null);
        }
    }
}
