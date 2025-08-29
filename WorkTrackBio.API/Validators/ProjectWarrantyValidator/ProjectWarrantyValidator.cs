using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Validators.ProjectWarrantyValidator
{
    /// <summary>
    /// Implementación del validador para ProjectWarranty
    /// </summary>
    public class ProjectWarrantyValidator : IProjectWarrantyValidator
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeInfoRepository _employeeRepository;
        private readonly IStateRepository _stateRepository;

        public ProjectWarrantyValidator(
            IProjectRepository projectRepository,
            IEmployeeInfoRepository employeeRepository,
            IStateRepository stateRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateProjectWarrantyDataTransferObject createDto)
        {
            var result = new ValidationResult();

            // Validar que el proyecto existe
            var projectValidation = await ValidateProjectExistsAsync(createDto.IdProject);
            if (!projectValidation.IsValid)
                result.Errors.AddRange(projectValidation.Errors);

            // Validar que el empleado existe
            var employeeValidation = await ValidateEmployeeExistsAsync(createDto.MadeById);
            if (!employeeValidation.IsValid)
                result.Errors.AddRange(employeeValidation.Errors);

            // Validar que el estado existe
            var stateValidation = await ValidateStateExistsAsync(createDto.StateId);
            if (!stateValidation.IsValid)
                result.Errors.AddRange(stateValidation.Errors);

            return result;
        }

        public async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto)
        {
            var result = new ValidationResult();

            // Validar que el proyecto existe si se está actualizando
            if (updateDto.IdProject.HasValue)
            {
                var projectValidation = await ValidateProjectExistsAsync(updateDto.IdProject.Value);
                if (!projectValidation.IsValid)
                    result.Errors.AddRange(projectValidation.Errors);
            }

            // Validar que el empleado existe si se está actualizando
            if (updateDto.MadeById.HasValue)
            {
                var employeeValidation = await ValidateEmployeeExistsAsync(updateDto.MadeById.Value);
                if (!employeeValidation.IsValid)
                    result.Errors.AddRange(employeeValidation.Errors);
            }

            // Validar que el estado existe si se está actualizando
            if (updateDto.StateId.HasValue)
            {
                var stateValidation = await ValidateStateExistsAsync(updateDto.StateId.Value);
                if (!stateValidation.IsValid)
                    result.Errors.AddRange(stateValidation.Errors);
            }

            return result;
        }

        public async Task<ValidationResult> ValidateProjectExistsAsync(int projectId)
        {
            var result = new ValidationResult();
            
            if (projectId <= 0)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("IdProject", "El ID del proyecto debe ser mayor que 0"));
                return result;
            }

            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("IdProject", $"No existe un proyecto con ID {projectId}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId)
        {
            var result = new ValidationResult();
            
            if (employeeId <= 0)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("MadeById", "El ID del empleado debe ser mayor que 0"));
                return result;
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("MadeById", $"No existe un empleado con ID {employeeId}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateStateExistsAsync(int stateId)
        {
            var result = new ValidationResult();
            
            if (stateId <= 0)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("StateId", "El ID del estado debe ser mayor que 0"));
                return result;
            }

            var state = await _stateRepository.GetByIdAsync(stateId);
            if (state == null)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("StateId", $"No existe un estado con ID {stateId}"));
            }

            return result;
        }
    }
}
