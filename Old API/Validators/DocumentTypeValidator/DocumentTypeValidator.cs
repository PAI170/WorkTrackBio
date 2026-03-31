using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.DocumentType;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;

namespace WorkTrackBio.API.Validators.DocumentTypeValidator
{
    public class DocumentTypeValidator : IDocumentTypeValidator
    {
        private readonly IValidator<CreateDocumentTypeDataTransferObject> _createValidator;
        private readonly IValidator<UpdateDocumentTypeDataTransferObject> _updateValidator;
        private readonly IDocumentTypeRepository _documentTypeRepository;

        public DocumentTypeValidator(IDocumentTypeRepository documentTypeRepository)
        {
            _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
            _createValidator = new CreateDocumentTypeValidator();
            _updateValidator = new UpdateDocumentTypeValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateDocumentTypeDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del tipo de documento no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var nameUniqueValidation = await ValidateDocumentTypeNameUniqueAsync(createDto.DocumentName);
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

        public async Task<ValidationResult> ValidateUpdateAsync(UpdateDocumentTypeDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del tipo de documento no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var existsValidation = await ValidateDocumentTypeExistsAsync(updateDto.Id);
            if (!existsValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in existsValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.DocumentName))
            {
                var nameUniqueValidation = await ValidateDocumentTypeNameUniqueAsync(updateDto.DocumentName, updateDto.Id);
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

        public ValidationResult ValidateDocumentName(string documentName)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(documentName))
            {
                result.Errors.Add(new ValidationFailure("DocumentName", "El nombre del tipo de documento no puede estar vacío"));
            }
            else if (documentName.Length > 50)
            {
                result.Errors.Add(new ValidationFailure("DocumentName", "El nombre del tipo de documento no puede exceder 50 caracteres"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateDocumentTypeExistsAsync(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
                return result;
            }

            try
            {
                var documentType = await _documentTypeRepository.GetByIdAsync(id);
                if (documentType == null)
                {
                    result.Errors.Add(new ValidationFailure("Id", $"No existe un tipo de documento con ID {id}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("Id", $"Error al validar la existencia del tipo de documento: {ex.Message}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateDocumentTypeNameUniqueAsync(string documentName, int? excludeId = null)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(documentName))
            {
                result.Errors.Add(new ValidationFailure("DocumentName", "El nombre del tipo de documento no puede estar vacío"));
                return result;
            }

            try
            {
                var exists = await _documentTypeRepository.ExistsByNameAsync(documentName);
                if (exists)
                {
                    if (excludeId.HasValue)
                    {
                        var allDocumentTypes = await _documentTypeRepository.GetAllAsync();
                        var conflictingDocumentType = allDocumentTypes.FirstOrDefault(dt => 
                            dt.DocumentName.Equals(documentName, StringComparison.OrdinalIgnoreCase) && 
                            dt.Id != excludeId.Value);
                        
                        if (conflictingDocumentType != null)
                        {
                            result.Errors.Add(new ValidationFailure("DocumentName", 
                                $"Ya existe otro tipo de documento con el nombre '{documentName}'"));
                        }
                    }
                    else
                    {
                        result.Errors.Add(new ValidationFailure("DocumentName", 
                            $"Ya existe un tipo de documento con el nombre '{documentName}'"));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("DocumentName", 
                    $"Error al validar la unicidad del nombre: {ex.Message}"));
            }

            return result;
        }
    }

    public class CreateDocumentTypeValidator : AbstractValidator<CreateDocumentTypeDataTransferObject>
    {
        public CreateDocumentTypeValidator()
        {
            RuleFor(x => x.DocumentName)
                .NotEmpty().WithMessage("El nombre del tipo de documento es obligatorio")
                .MaximumLength(50).WithMessage("El nombre del tipo de documento no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El nombre del tipo de documento no puede estar vacío o contener solo espacios");

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede exceder 255 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }

    public class UpdateDocumentTypeValidator : AbstractValidator<UpdateDocumentTypeDataTransferObject>
    {
        public UpdateDocumentTypeValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor que 0")
                .LessThan(int.MaxValue).WithMessage("El ID excede el valor máximo permitido");

            RuleFor(x => x.DocumentName)
                .MaximumLength(50).WithMessage("El nombre del tipo de documento no puede exceder 50 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un nombre, no puede estar vacío o contener solo espacios")
                .When(x => x.DocumentName != null);

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage("La descripción no puede exceder 255 caracteres (restricción de BD)")
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
