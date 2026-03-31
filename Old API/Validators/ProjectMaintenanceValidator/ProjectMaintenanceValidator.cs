using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;
using WorkTrackBio.API.Repositories.ProjectMaintenanceRepository;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Validators.ProjectMaintenanceValidator
{
    public class ProjectMaintenanceValidator : IProjectMaintenanceValidator
    {
        private readonly IValidator<CreateProjectMaintenanceDataTransferObject> _createValidator;
        private readonly IValidator<UpdateProjectMaintenanceDataTransferObject> _updateValidator;
        private readonly IProjectMaintenanceRepository _projectMaintenanceRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IStateRepository _stateRepository;

        public ProjectMaintenanceValidator(
            IProjectMaintenanceRepository projectMaintenanceRepository,
            IProjectRepository projectRepository,
            IEmployeeInfoRepository employeeInfoRepository,
            IStateRepository stateRepository)
        {
            _projectMaintenanceRepository = projectMaintenanceRepository ?? throw new ArgumentNullException(nameof(projectMaintenanceRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _employeeInfoRepository = employeeInfoRepository ?? throw new ArgumentNullException(nameof(employeeInfoRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _createValidator = new CreateProjectMaintenanceValidator();
            _updateValidator = new UpdateProjectMaintenanceValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateProjectMaintenanceDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del mantenimiento no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var projectValidation = await ValidateProjectExistsAsync(createDto.IdProject);
            if (!projectValidation.IsValid)
                return projectValidation;

            var employeeValidation = await ValidateEmployeeExistsAsync(createDto.MadeById);
            if (!employeeValidation.IsValid)
                return employeeValidation;

            var stateValidation = await ValidateStateExistsAsync(createDto.StateId);
            if (!stateValidation.IsValid)
                return stateValidation;

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateUpdateAsync(UpdateProjectMaintenanceDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("", "Los datos del mantenimiento no pueden estar vacíos"));
                return result;
            }

            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            var existsValidation = await ValidateMaintenanceExistsAsync(updateDto.Id);
            if (!existsValidation.IsValid)
                return existsValidation;

            if (updateDto.IdProject.HasValue)
            {
                var projectValidation = await ValidateProjectExistsAsync(updateDto.IdProject.Value);
                if (!projectValidation.IsValid)
                    return projectValidation;
            }

            if (updateDto.MadeById.HasValue)
            {
                var employeeValidation = await ValidateEmployeeExistsAsync(updateDto.MadeById.Value);
                if (!employeeValidation.IsValid)
                    return employeeValidation;
            }

            if (updateDto.StateId.HasValue)
            {
                var stateValidation = await ValidateStateExistsAsync(updateDto.StateId.Value);
                if (!stateValidation.IsValid)
                    return stateValidation;
            }

            return new ValidationResult();
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

        public ValidationResult ValidateProjectId(int projectId)
        {
            var result = new ValidationResult();
            
            if (projectId <= 0)
            {
                result.Errors.Add(new ValidationFailure("IdProject", "El ID del proyecto debe ser mayor que 0"));
            }

            return result;
        }

        public ValidationResult ValidateEmployeeId(int employeeId)
        {
            var result = new ValidationResult();
            
            if (employeeId <= 0)
            {
                result.Errors.Add(new ValidationFailure("MadeById", "El ID del empleado debe ser mayor que 0"));
            }

            return result;
        }

        public ValidationResult ValidateStateId(int stateId)
        {
            var result = new ValidationResult();
            
            if (stateId <= 0)
            {
                result.Errors.Add(new ValidationFailure("StateId", "El ID del estado debe ser mayor que 0"));
            }

            return result;
        }

        public ValidationResult ValidateMaintenanceDescription(string description)
        {
            var result = new ValidationResult();
            
            if (string.IsNullOrWhiteSpace(description))
            {
                result.Errors.Add(new ValidationFailure("MaintenanceDescription", "La descripción del mantenimiento no puede estar vacía"));
            }
            else if (description.Length > 500)
            {
                result.Errors.Add(new ValidationFailure("MaintenanceDescription", "La descripción del mantenimiento no puede exceder 500 caracteres"));
            }

            return result;
        }

        public ValidationResult ValidateMaintenanceCost(decimal? cost)
        {
            var result = new ValidationResult();
            
            if (cost.HasValue && cost.Value < 0)
            {
                result.Errors.Add(new ValidationFailure("MaintenanceCost", "El costo del mantenimiento no puede ser negativo"));
            }
            else if (cost.HasValue && cost.Value > 999999.99m)
            {
                result.Errors.Add(new ValidationFailure("MaintenanceCost", "El costo del mantenimiento no puede exceder 999,999.99"));
            }

            return result;
        }

        public ValidationResult ValidateAdditionalInfo(string? additionalInfo)
        {
            var result = new ValidationResult();
            
            if (!string.IsNullOrWhiteSpace(additionalInfo) && additionalInfo.Length > 255)
            {
                result.Errors.Add(new ValidationFailure("AdditionalInfo", "La información adicional no puede exceder 255 caracteres"));
            }

            return result;
        }

        private async Task<ValidationResult> ValidateMaintenanceExistsAsync(int id)
        {
            var result = new ValidationResult();
            
            var maintenanceExists = await _projectMaintenanceRepository.ExistsByIdAsync(id);
            if (!maintenanceExists)
            {
                result.Errors.Add(new ValidationFailure("Id", $"No existe un mantenimiento con ID {id}"));
            }

            return result;
        }

        private async Task<ValidationResult> ValidateProjectExistsAsync(int projectId)
        {
            var result = new ValidationResult();
            
            var projectExists = await _projectRepository.GetByIdAsync(projectId);
            if (projectExists == null)
            {
                result.Errors.Add(new ValidationFailure("IdProject", $"No existe un proyecto con ID {projectId}"));
            }

            return result;
        }

        private async Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId)
        {
            var result = new ValidationResult();
            
            var employeeExists = await _employeeInfoRepository.GetByIdAsync(employeeId);
            if (employeeExists == null)
            {
                result.Errors.Add(new ValidationFailure("MadeById", $"No existe un empleado con ID {employeeId}"));
            }

            return result;
        }

        private async Task<ValidationResult> ValidateStateExistsAsync(int stateId)
        {
            var result = new ValidationResult();
            
            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
            {
                result.Errors.Add(new ValidationFailure("StateId", $"No existe un estado con ID {stateId}"));
            }

            return result;
        }

        // Validadores internos para los DTOs
        private class CreateProjectMaintenanceValidator : AbstractValidator<CreateProjectMaintenanceDataTransferObject>
        {
            public CreateProjectMaintenanceValidator()
            {
                RuleFor(x => x.IdProject)
                    .GreaterThan(0).WithMessage("El ID del proyecto debe ser mayor que 0");

                RuleFor(x => x.MaintenanceDescription)
                    .NotEmpty().WithMessage("La descripción del mantenimiento es obligatoria")
                    .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

                RuleFor(x => x.MadeById)
                    .GreaterThan(0).WithMessage("El ID del empleado es obligatorio");

                RuleFor(x => x.MaintenanceCost)
                    .InclusiveBetween(0, 999999.99m).WithMessage("El costo debe estar entre 0 y 999,999.99")
                    .When(x => x.MaintenanceCost.HasValue);

                RuleFor(x => x.AdditionalInfo)
                    .MaximumLength(255).WithMessage("La información adicional no puede exceder 255 caracteres")
                    .When(x => !string.IsNullOrWhiteSpace(x.AdditionalInfo));

                RuleFor(x => x.StateId)
                    .GreaterThan(0).WithMessage("El ID del estado es obligatorio");
            }
        }

        private class UpdateProjectMaintenanceValidator : AbstractValidator<UpdateProjectMaintenanceDataTransferObject>
        {
            public UpdateProjectMaintenanceValidator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0).WithMessage("El ID debe ser mayor que 0");

                RuleFor(x => x.IdProject)
                    .GreaterThan(0).WithMessage("El ID del proyecto debe ser mayor que 0")
                    .When(x => x.IdProject.HasValue);

                RuleFor(x => x.MaintenanceDescription)
                    .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres")
                    .When(x => !string.IsNullOrWhiteSpace(x.MaintenanceDescription));

                RuleFor(x => x.MadeById)
                    .GreaterThan(0).WithMessage("El ID del empleado debe ser mayor que 0")
                    .When(x => x.MadeById.HasValue);

                RuleFor(x => x.MaintenanceCost)
                    .InclusiveBetween(0, 999999.99m).WithMessage("El costo debe estar entre 0 y 999,999.99")
                    .When(x => x.MaintenanceCost.HasValue);

                RuleFor(x => x.AdditionalInfo)
                    .MaximumLength(255).WithMessage("La información adicional no puede exceder 255 caracteres")
                    .When(x => !string.IsNullOrWhiteSpace(x.AdditionalInfo));

                RuleFor(x => x.StateId)
                    .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0")
                    .When(x => x.StateId.HasValue);
            }
        }
    }
}
