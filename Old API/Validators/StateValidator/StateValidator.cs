using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.State;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Validators.StateValidator
{
    public class StateValidator : IStateValidator
    {
        private readonly IValidator<CreateStateDataTransferObject> _createValidator;
        private readonly IValidator<UpdateStateDataTransferObject> _updateValidator;
        private readonly IStateRepository _stateRepository;
        public StateValidator(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _createValidator = new CreateStateValidator();
            _updateValidator = new UpdateStateValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateStateDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del estado no pueden estar vacíos"));
                return result;
            }

            // Validaciones básicas de formato
            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            // Validación de unicidad del nombre
            var nameUniqueValidation = await ValidateStateNameUniqueAsync(createDto.StateName);
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

        public async Task<ValidationResult> ValidateUpdateAsync(UpdateStateDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del estado no pueden estar vacíos"));
                return result;
            }

            // Validaciones básicas de formato
            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            // Validación de existencia del ID
            var existsValidation = await ValidateStateExistsAsync(updateDto.Id);
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
            if (!string.IsNullOrWhiteSpace(updateDto.StateName))
            {
                var nameUniqueValidation = await ValidateStateNameUniqueAsync(updateDto.StateName, updateDto.Id);
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
        public ValidationResult ValidateStateName(string stateName)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(stateName))
            {
                result.Errors.Add(new ValidationFailure("StateName", "El nombre del estado no puede estar vacío"));
            }
            else if (stateName.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("StateName", "El nombre del estado no puede exceder 50 caracteres"));
            }

            return result;
        }
        public ValidationResult ValidateStateType(string stateType)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(stateType))
            {
                result.Errors.Add(new ValidationFailure("StateType", "El tipo de estado no puede estar vacío"));
            }
            else if (stateType.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("StateType", "El tipo de estado no puede exceder 50 caracteres (restricción de BD)"));
            }
            else
            {
                // Validar que no sea solo espacios (según CHECK constraint de BD)
                if (string.IsNullOrWhiteSpace(stateType.Trim()))
                {
                    result.Errors.Add(new ValidationFailure("StateType", 
                        "El tipo de estado no puede estar vacío o contener solo espacios (restricción de BD)"));
                }
            }

            return result;
        }
        public async Task<ValidationResult> ValidateStateExistsAsync(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
                return result;
            }

            try
            {
                var state = await _stateRepository.GetByIdAsync(id);
                if (state == null)
                {
                    result.Errors.Add(new ValidationFailure("Id", $"No existe un estado con ID {id}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("Id", $"Error al validar la existencia del estado: {ex.Message}"));
            }

            return result;
        }
        public async Task<ValidationResult> ValidateStateNameUniqueAsync(string stateName, int? excludeId = null)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(stateName))
            {
                result.Errors.Add(new ValidationFailure("StateName", "El nombre del estado no puede estar vacío"));
                return result;
            }

            try
            {
                var exists = await _stateRepository.ExistsByNameAsync(stateName);
                if (exists)
                {
                    // Si estamos actualizando, verificar si el nombre pertenece al mismo estado
                    if (excludeId.HasValue)
                    {
                        var allStates = await _stateRepository.GetAllAsync();
                        var conflictingState = allStates.FirstOrDefault(s => 
                            s.StateName.Equals(stateName, StringComparison.OrdinalIgnoreCase) && 
                            s.Id != excludeId.Value);
                        
                        if (conflictingState != null)
                        {
                            result.Errors.Add(new ValidationFailure("StateName", 
                                $"Ya existe otro estado con el nombre '{stateName}'"));
                        }
                    }
                    else
                    {
                        // Para creación, cualquier duplicado es inválido
                        result.Errors.Add(new ValidationFailure("StateName", 
                            $"Ya existe un estado con el nombre '{stateName}'"));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("StateName", 
                    $"Error al validar la unicidad del nombre: {ex.Message}"));
            }

            return result;
        }
    }
    public class CreateStateValidator : AbstractValidator<CreateStateDataTransferObject>
    {
        public CreateStateValidator()
        {
            RuleFor(x => x.StateName)
                .NotEmpty().WithMessage("El nombre del estado es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del estado no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El nombre del estado no puede estar vacío o contener solo espacios (restricción de BD)");

            RuleFor(x => x.StateType)
                .NotEmpty().WithMessage("El tipo de estado es obligatorio")
                .MaximumLength(50).WithMessage("El tipo de estado no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El tipo de estado no puede estar vacío o contener solo espacios (restricción de BD)");

            RuleFor(x => x.Description)
                .MaximumLength(50).WithMessage("La descripción no puede exceder 50 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
    public class UpdateStateValidator : AbstractValidator<UpdateStateDataTransferObject>
    {
        public UpdateStateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor que 0")
                .LessThan(int.MaxValue).WithMessage("El ID excede el valor máximo permitido");

            // StateName es opcional en updates - solo validar si se proporciona
            RuleFor(x => x.StateName)
                .MaximumLength(50).WithMessage("El nombre del estado no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un nombre, no puede estar vacío o contener solo espacios")
                .When(x => x.StateName != null);

            // StateType es opcional en updates - solo validar si se proporciona
            RuleFor(x => x.StateType)
                .MaximumLength(50).WithMessage("El tipo de estado no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un tipo, no puede estar vacío o contener solo espacios")
                .When(x => x.StateType != null);

            // Description es opcional en updates - solo validar si se proporciona
            RuleFor(x => x.Description)
                .MaximumLength(50).WithMessage("La descripción no puede exceder 50 caracteres (restricción de BD)")
                .When(x => x.Description != null);
        }
    }
}
