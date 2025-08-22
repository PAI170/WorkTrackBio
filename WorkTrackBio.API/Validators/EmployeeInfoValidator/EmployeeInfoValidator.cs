using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;

namespace WorkTrackBio.API.Validators.EmployeeInfoValidator
{
    public class EmployeeInfoValidator : IEmployeeInfoValidator
    {
        private readonly IValidator<CreateEmployeeInfoDataTransferObject> _createValidator;
        private readonly IValidator<UpdateEmployeeInfoDataTransferObject> _updateValidator;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;

        public EmployeeInfoValidator(
            IEmployeeInfoRepository employeeInfoRepository,
            IStateRepository stateRepository,
            IDocumentTypeRepository documentTypeRepository)
        {
            _employeeInfoRepository = employeeInfoRepository ?? throw new ArgumentNullException(nameof(employeeInfoRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
            _createValidator = new CreateEmployeeInfoValidator();
            _updateValidator = new UpdateEmployeeInfoValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateEmployeeInfoDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del empleado no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var documentNumberUniqueValidation = await ValidateDocumentNumberUniqueAsync(createDto.DocumentNumber, createDto.DocumentTypeId);
            if (!documentNumberUniqueValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in documentNumberUniqueValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            var stateValidation = await ValidateStateExistsAsync(createDto.StateId);
            if (!stateValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in stateValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            var documentTypeValidation = await ValidateDocumentTypeExistsAsync(createDto.DocumentTypeId);
            if (!documentTypeValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in documentTypeValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            var datesValidation = ValidateDates(createDto.DocumentExpire, createDto.Birthday);
            if (!datesValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in datesValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            var costValidation = ValidateCostPerHour(createDto.CostPerHour);
            if (!costValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in costValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            return basicValidation;
        }

        public async Task<ValidationResult> ValidateUpdateAsync(UpdateEmployeeInfoDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del empleado no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var existsValidation = await ValidateEmployeeExistsAsync(updateDto.Id);
            if (!existsValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in existsValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.DocumentNumber))
            {
                var documentTypeId = updateDto.DocumentTypeId ?? 0;
                if (documentTypeId > 0)
                {
                    var documentNumberUniqueValidation = await ValidateDocumentNumberUniqueAsync(updateDto.DocumentNumber, documentTypeId, updateDto.Id);
                    if (!documentNumberUniqueValidation.IsValid)
                    {
                        var combinedResult = new ValidationResult();
                        foreach (var error in basicValidation.Errors)
                            combinedResult.Errors.Add(error);
                        foreach (var error in documentNumberUniqueValidation.Errors)
                            combinedResult.Errors.Add(error);
                        return combinedResult;
                    }
                }
            }

            if (updateDto.StateId.HasValue)
            {
                var stateValidation = await ValidateStateExistsAsync(updateDto.StateId.Value);
                if (!stateValidation.IsValid)
                {
                    var combinedResult = new ValidationResult();
                    foreach (var error in basicValidation.Errors)
                        combinedResult.Errors.Add(error);
                    foreach (var error in stateValidation.Errors)
                        combinedResult.Errors.Add(error);
                    return combinedResult;
                }
            }

            if (updateDto.DocumentTypeId.HasValue)
            {
                var documentTypeValidation = await ValidateDocumentTypeExistsAsync(updateDto.DocumentTypeId.Value);
                if (!documentTypeValidation.IsValid)
                {
                    var combinedResult = new ValidationResult();
                    foreach (var error in basicValidation.Errors)
                        combinedResult.Errors.Add(error);
                    foreach (var error in documentTypeValidation.Errors)
                        combinedResult.Errors.Add(error);
                    return combinedResult;
                }
            }

            var datesValidation = ValidateDates(updateDto.DocumentExpire, updateDto.Birthday);
            if (!datesValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in datesValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            var costValidation = ValidateCostPerHour(updateDto.CostPerHour);
            if (!costValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in costValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
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

        public ValidationResult ValidateDocumentNumber(string documentNumber)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                result.Errors.Add(new ValidationFailure("DocumentNumber", "El número de documento no puede estar vacío"));
            }
            else if (documentNumber.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("DocumentNumber", "El número de documento no puede exceder 50 caracteres"));
            }

            return result;
        }

        public ValidationResult ValidateNames(string firstName, string lastName)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(firstName))
            {
                result.Errors.Add(new ValidationFailure("FirstName", "El nombre no puede estar vacío"));
            }
            else if (firstName.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("FirstName", "El nombre no puede exceder 50 caracteres"));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                result.Errors.Add(new ValidationFailure("LastName", "El apellido no puede estar vacío"));
            }
            else if (lastName.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("LastName", "El apellido no puede exceder 50 caracteres"));
            }

            return result;
        }

        public ValidationResult ValidatePhoneNumber(string? phoneNumber)
        {
            var result = new ValidationResult();
            
            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length > 20)
            {
                result.Errors.Add(new ValidationFailure("PhoneNumber", "El número de teléfono no puede exceder 20 caracteres"));
            }

            return result;
        }

        public ValidationResult ValidateDates(DateOnly? documentExpire, DateOnly? birthday)
        {
            var result = new ValidationResult();
            
            if (documentExpire.HasValue && documentExpire.Value < DateOnly.FromDateTime(DateTime.Today))
            {
                result.Errors.Add(new ValidationFailure("DocumentExpire", "La fecha de expiración del documento no puede ser anterior a hoy"));
            }

            if (birthday.HasValue)
            {
                if (birthday.Value > DateOnly.FromDateTime(DateTime.Today))
                {
                    result.Errors.Add(new ValidationFailure("Birthday", "La fecha de nacimiento no puede ser futura"));
                }
                else if (birthday.Value < DateOnly.FromDateTime(DateTime.Today.AddYears(-100)))
                {
                    result.Errors.Add(new ValidationFailure("Birthday", "La fecha de nacimiento no puede ser anterior a 100 años"));
                }
            }

            return result;
        }

        public ValidationResult ValidateCostPerHour(decimal? costPerHour)
        {
            var result = new ValidationResult();
            
            if (costPerHour.HasValue && costPerHour.Value < 0)
            {
                result.Errors.Add(new ValidationFailure("CostPerHour", "El costo por hora no puede ser negativo"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateEmployeeExistsAsync(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
                return result;
            }

            try
            {
                var employee = await _employeeInfoRepository.GetByIdAsync(id);
                if (employee == null)
                {
                    result.Errors.Add(new ValidationFailure("Id", $"No existe un empleado con ID {id}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("Id", $"Error al validar la existencia del empleado: {ex.Message}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateDocumentNumberUniqueAsync(string documentNumber, int documentTypeId, int? excludeId = null)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                result.Errors.Add(new ValidationFailure("DocumentNumber", "El número de documento no puede estar vacío"));
                return result;
            }

            if (documentTypeId <= 0)
            {
                result.Errors.Add(new ValidationFailure("DocumentTypeId", "El ID del tipo de documento debe ser mayor que 0"));
                return result;
            }

            try
            {
                var exists = await _employeeInfoRepository.ExistsByDocumentNumberAsync(documentNumber, documentTypeId);
                if (exists)
                {
                    if (excludeId.HasValue)
                    {
                        var allEmployees = await _employeeInfoRepository.GetAllAsync();
                        var conflictingEmployee = allEmployees.FirstOrDefault(e => 
                            e.DocumentNumber.Equals(documentNumber, StringComparison.OrdinalIgnoreCase) && 
                            e.DocumentTypeId == documentTypeId && 
                            e.Id != excludeId.Value);
                        
                        if (conflictingEmployee != null)
                        {
                            result.Errors.Add(new ValidationFailure("DocumentNumber", 
                                $"Ya existe otro empleado con el número de documento '{documentNumber}' del mismo tipo"));
                        }
                    }
                    else
                    {
                        result.Errors.Add(new ValidationFailure("DocumentNumber", 
                            $"Ya existe un empleado con el número de documento '{documentNumber}' del mismo tipo"));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("DocumentNumber", 
                    $"Error al validar la unicidad del número de documento: {ex.Message}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateStateExistsAsync(int stateId)
        {
            var result = new ValidationResult();
            
            if (stateId <= 0)
            {
                result.Errors.Add(new ValidationFailure("StateId", "El ID del estado debe ser mayor que 0"));
                return result;
            }

            try
            {
                var state = await _stateRepository.GetByIdAsync(stateId);
                if (state == null)
                {
                    result.Errors.Add(new ValidationFailure("StateId", $"No existe un estado con ID {stateId}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("StateId", 
                    $"Error al validar la existencia del estado: {ex.Message}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateDocumentTypeExistsAsync(int documentTypeId)
        {
            var result = new ValidationResult();
            
            if (documentTypeId <= 0)
            {
                result.Errors.Add(new ValidationFailure("DocumentTypeId", "El ID del tipo de documento debe ser mayor que 0"));
                return result;
            }

            try
            {
                var documentType = await _documentTypeRepository.GetByIdAsync(documentTypeId);
                if (documentType == null)
                {
                    result.Errors.Add(new ValidationFailure("DocumentTypeId", $"No existe un tipo de documento con ID {documentTypeId}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("DocumentTypeId", 
                    $"Error al validar la existencia del tipo de documento: {ex.Message}"));
            }

            return result;
        }
    }

    public class CreateEmployeeInfoValidator : AbstractValidator<CreateEmployeeInfoDataTransferObject>
    {
        public CreateEmployeeInfoValidator()
        {
            RuleFor(x => x.DocumentNumber)
                .NotEmpty().WithMessage("El número de documento es obligatorio")
                .MaximumLength(50).WithMessage("El número de documento no puede exceder 50 caracteres (restricción de BD)")
                .Must(number => !string.IsNullOrWhiteSpace(number?.Trim())).WithMessage("El número de documento no puede estar vacío o contener solo espacios");

            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0).WithMessage("El ID del tipo de documento debe ser mayor que 0");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("El nombre es obligatorio")
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El nombre no puede estar vacío o contener solo espacios");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido es obligatorio")
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El apellido no puede estar vacío o contener solo espacios");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.EmergencyContact)
                .MaximumLength(100).WithMessage("El contacto de emergencia no puede exceder 100 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContact));

            RuleFor(x => x.EmergencyContactPhoneNumber)
                .MaximumLength(20).WithMessage("El teléfono de contacto de emergencia no puede exceder 20 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhoneNumber));

            RuleFor(x => x.Birthday)
                .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria")
                .Must(date => date <= DateOnly.FromDateTime(DateTime.Today)).WithMessage("La fecha de nacimiento no puede ser futura")
                .Must(date => date >= DateOnly.FromDateTime(DateTime.Today.AddYears(-100))).WithMessage("La fecha de nacimiento no puede ser anterior a 100 años");

            RuleFor(x => x.CostPerHour)
                .GreaterThanOrEqualTo(0).WithMessage("El costo por hora no puede ser negativo")
                .When(x => x.CostPerHour.HasValue);

            RuleFor(x => x.StateId)
                .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0");

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("La dirección no puede exceder 255 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.IBAN)
                .MaximumLength(50).WithMessage("El IBAN no puede exceder 50 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.IBAN));
        }
    }

    public class UpdateEmployeeInfoValidator : AbstractValidator<UpdateEmployeeInfoDataTransferObject>
    {
        public UpdateEmployeeInfoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor que 0")
                .LessThan(int.MaxValue).WithMessage("El ID excede el valor máximo permitido");

            RuleFor(x => x.DocumentNumber)
                .MaximumLength(50).WithMessage("El número de documento no puede exceder 50 caracteres (restricción de BD)")
                .Must(number => number == null || !string.IsNullOrWhiteSpace(number.Trim())).WithMessage("Si se proporciona un número de documento, no puede estar vacío o contener solo espacios")
                .When(x => x.DocumentNumber != null);

            RuleFor(x => x.DocumentTypeId)
                .GreaterThan(0).WithMessage("El ID del tipo de documento debe ser mayor que 0")
                .When(x => x.DocumentTypeId.HasValue);

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un nombre, no puede estar vacío o contener solo espacios")
                .When(x => x.FirstName != null);

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un apellido, no puede estar vacío o contener solo espacios")
                .When(x => x.LastName != null);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("El número de teléfono no puede exceder 20 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.EmergencyContact)
                .MaximumLength(100).WithMessage("El contacto de emergencia no puede exceder 100 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContact));

            RuleFor(x => x.EmergencyContactPhoneNumber)
                .MaximumLength(20).WithMessage("El teléfono de contacto de emergencia no puede exceder 20 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhoneNumber));

            RuleFor(x => x.Birthday)
                .Must(date => !date.HasValue || date.Value <= DateOnly.FromDateTime(DateTime.Today)).WithMessage("La fecha de nacimiento no puede ser futura")
                .Must(date => !date.HasValue || date.Value >= DateOnly.FromDateTime(DateTime.Today.AddYears(-100))).WithMessage("La fecha de nacimiento no puede ser anterior a 100 años")
                .When(x => x.Birthday.HasValue);

            RuleFor(x => x.CostPerHour)
                .GreaterThanOrEqualTo(0).WithMessage("El costo por hora no puede ser negativo")
                .When(x => x.CostPerHour.HasValue);

            RuleFor(x => x.StateId)
                .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0")
                .When(x => x.StateId.HasValue);

            RuleFor(x => x.Address)
                .MaximumLength(255).WithMessage("La dirección no puede exceder 255 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.IBAN)
                .MaximumLength(50).WithMessage("El IBAN no puede exceder 50 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.IBAN));
        }
    }
}
