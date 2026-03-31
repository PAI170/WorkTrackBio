using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Project;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Validators.ProjectValidator
{
    public class ProjectValidator : IProjectValidator
    {
        private readonly IValidator<CreateProjectDataTransferObject> _createValidator;
        private readonly IValidator<UpdateProjectDataTransferObject> _updateValidator;
        private readonly IProjectRepository _projectRepository;
        private readonly IStateRepository _stateRepository;

        public ProjectValidator(IProjectRepository projectRepository, IStateRepository stateRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _createValidator = new CreateProjectValidator();
            _updateValidator = new UpdateProjectValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateProjectDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del proyecto no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var nameUniqueValidation = await ValidateProjectNameUniqueAsync(createDto.ProjectName);
            if (!nameUniqueValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in nameUniqueValidation.Errors)
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

            var datesValidation = ValidateDates(createDto.StartDate, createDto.EndDate);
            if (!datesValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in datesValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            return basicValidation;
        }

        public async Task<ValidationResult> ValidateUpdateAsync(UpdateProjectDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del proyecto no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var existsValidation = await ValidateProjectExistsAsync(updateDto.Id);
            if (!existsValidation.IsValid)
            {
                var combinedResult = new ValidationResult();
                foreach (var error in basicValidation.Errors)
                    combinedResult.Errors.Add(error);
                foreach (var error in existsValidation.Errors)
                    combinedResult.Errors.Add(error);
                return combinedResult;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.ProjectName))
            {
                var nameUniqueValidation = await ValidateProjectNameUniqueAsync(updateDto.ProjectName, updateDto.Id);
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

            if (updateDto.StartDate.HasValue || updateDto.EndDate.HasValue)
            {
                var datesValidation = ValidateDates(updateDto.StartDate, updateDto.EndDate);
                if (!datesValidation.IsValid)
                {
                    var combinedResult = new ValidationResult();
                    foreach (var error in basicValidation.Errors)
                        combinedResult.Errors.Add(error);
                    foreach (var error in datesValidation.Errors)
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

        public ValidationResult ValidateProjectName(string projectName)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(projectName))
            {
                result.Errors.Add(new ValidationFailure("ProjectName", "El nombre del proyecto no puede estar vacío"));
            }
            else if (projectName.Length > 150)
            {
                result.Errors.Add(new ValidationFailure("ProjectName", "El nombre del proyecto no puede exceder 150 caracteres"));
            }

            return result;
        }

        public ValidationResult ValidateDates(DateOnly? startDate, DateOnly? endDate)
        {
            var result = new ValidationResult();
            
            if (startDate.HasValue && endDate.HasValue)
            {
                if (endDate.Value < startDate.Value)
                {
                    result.Errors.Add(new ValidationFailure("EndDate", "La fecha de finalización no puede ser anterior a la fecha de inicio"));
                }
            }

            return result;
        }

        public async Task<ValidationResult> ValidateProjectExistsAsync(int id)
        {
            var result = new ValidationResult();
            
            if (id <= 0)
            {
                result.Errors.Add(new ValidationFailure("Id", "El ID debe ser mayor que 0"));
                return result;
            }

            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    result.Errors.Add(new ValidationFailure("Id", $"No existe un proyecto con ID {id}"));
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("Id", $"Error al validar la existencia del proyecto: {ex.Message}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateProjectNameUniqueAsync(string projectName, int? excludeId = null)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(projectName))
            {
                result.Errors.Add(new ValidationFailure("ProjectName", "El nombre del proyecto no puede estar vacío"));
                return result;
            }

            try
            {
                var exists = await _projectRepository.ExistsByNameAsync(projectName);
                if (exists)
                {

                    if (excludeId.HasValue)
                    {
                        var allProjects = await _projectRepository.GetAllAsync();
                        var conflictingProject = allProjects.FirstOrDefault(p => 
                            p.ProjectName.Equals(projectName, StringComparison.OrdinalIgnoreCase) && 
                            p.Id != excludeId.Value);
                        
                        if (conflictingProject != null)
                        {
                            result.Errors.Add(new ValidationFailure("ProjectName", 
                                $"Ya existe otro proyecto con el nombre '{projectName}'"));
                        }
                    }
                    else
                    {
                        result.Errors.Add(new ValidationFailure("ProjectName", 
                            $"Ya existe un proyecto con el nombre '{projectName}'"));
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ValidationFailure("ProjectName", 
                    $"Error al validar la unicidad del nombre: {ex.Message}"));
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
    }

    public class CreateProjectValidator : AbstractValidator<CreateProjectDataTransferObject>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("El nombre del proyecto es obligatorio")
                .MaximumLength(150).WithMessage("El nombre del proyecto no puede exceder 150 caracteres (restricción de BD)")
                .Must(name => !string.IsNullOrWhiteSpace(name?.Trim())).WithMessage("El nombre del proyecto no puede estar vacío o contener solo espacios");

            RuleFor(x => x.StateId)
                .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0");

            RuleFor(x => x.StartDate)
                .Must(date => !date.HasValue || date.Value <= DateOnly.FromDateTime(DateTime.Today.AddYears(100)))
                .WithMessage("La fecha de inicio no puede ser más de 100 años en el futuro");

            RuleFor(x => x.EndDate)
                .Must(date => !date.HasValue || date.Value <= DateOnly.FromDateTime(DateTime.Today.AddYears(100)))
                .WithMessage("La fecha de finalización no puede ser más de 100 años en el futuro");
        }
    }

    public class UpdateProjectValidator : AbstractValidator<UpdateProjectDataTransferObject>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID debe ser mayor que 0")
                .LessThan(int.MaxValue).WithMessage("El ID excede el valor máximo permitido");

            RuleFor(x => x.ProjectName)
                .MaximumLength(150).WithMessage("El nombre del proyecto no puede exceder 150 caracteres (restricción de BD)")
                .Must(name => name == null || !string.IsNullOrWhiteSpace(name.Trim())).WithMessage("Si se proporciona un nombre, no puede estar vacío o contener solo espacios")
                .When(x => x.ProjectName != null);

            RuleFor(x => x.StateId)
                .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0")
                .When(x => x.StateId.HasValue);

            RuleFor(x => x.StartDate)
                .Must(date => !date.HasValue || date.Value <= DateOnly.FromDateTime(DateTime.Today.AddYears(100)))
                .WithMessage("La fecha de inicio no puede ser más de 100 años en el futuro")
                .When(x => x.StartDate.HasValue);

            RuleFor(x => x.EndDate)
                .Must(date => !date.HasValue || date.Value <= DateOnly.FromDateTime(DateTime.Today.AddYears(100)))
                .WithMessage("La fecha de finalización no puede ser más de 100 años en el futuro")
                .When(x => x.EndDate.HasValue);
        }
    }
}
