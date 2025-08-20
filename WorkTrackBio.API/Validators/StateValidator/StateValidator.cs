using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.State;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Validators.StateValidator
{
    /// <summary>
    /// Validator para la entidad State usando FluentValidation con validaciones de base de datos
    /// </summary>
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

        /// <summary>
        /// Valida los datos para crear un estado
        /// Incluye validación de duplicados de nombre
        /// </summary>
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

        /// <summary>
        /// Valida los datos para actualizar un estado
        /// Incluye validación de existencia del ID y duplicados de nombre
        /// </summary>
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

            return basicValidation;
        }

        /// <summary>
        /// Valida el ID de un estado
        /// </summary>
        public ValidationResult ValidateId(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
            }

            return result;
        }

        /// <summary>
        /// Valida el nombre de un estado
        /// </summary>
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

        /// <summary>
        /// Valida el tipo de estado según las restricciones reales de la BD
        /// </summary>
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

        /// <summary>
        /// Valida si un ID de estado existe en la base de datos
        /// </summary>
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

        /// <summary>
        /// Valida si un nombre de estado ya existe (para evitar duplicados)
        /// </summary>
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

    /// <summary>
    /// Validator específico para crear estados
    /// Valida formato y restricciones de base de datos
    /// </summary>
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

    /// <summary>
    /// Validator específico para actualizar estados
    /// Valida formato, restricciones de base de datos y existencia
    /// </summary>
    public class UpdateStateValidator : AbstractValidator<UpdateStateDataTransferObject>
    {
        public UpdateStateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor que 0")
                .LessThan(int.MaxValue).WithMessage("El ID excede el valor máximo permitido");

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
}
