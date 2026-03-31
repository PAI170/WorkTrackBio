using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.Assistance;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Repositories.AssistanceRepository;

namespace WorkTrackBio.API.Validators.AssistanceValidator
{
    public class AssistanceValidator : IAssistanceValidator
    {
        private readonly IEmployeeInfoRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IAssistanceRepository _assistanceRepository;

        public AssistanceValidator(
            IEmployeeInfoRepository employeeRepository,
            IProjectRepository projectRepository,
            IAssistanceRepository assistanceRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _assistanceRepository = assistanceRepository ?? throw new ArgumentNullException(nameof(assistanceRepository));
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateAssistanceDataTransferObject createDto)
        {
            var validator = new CreateAssistanceValidator();
            var result = await validator.ValidateAsync(createDto);

            if (!result.IsValid)
                return result;

            // Validaciones adicionales de negocio
            var employeeValidation = await ValidateEmployeeExistsAsync(createDto.EmployeeId);
            if (!employeeValidation.IsValid)
                result.Errors.AddRange(employeeValidation.Errors);

            var projectValidation = await ValidateProjectExistsAsync(createDto.ProjectId);
            if (!projectValidation.IsValid)
                result.Errors.AddRange(projectValidation.Errors);

            var checkInOutValidation = await ValidateCheckInOutLogicAsync(createDto.EmployeeId, createDto.CheckOut);
            if (!checkInOutValidation.IsValid)
                result.Errors.AddRange(checkInOutValidation.Errors);

            return result;
        }

        public async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateAssistanceDataTransferObject updateDto)
        {
            var validator = new UpdateAssistanceValidator();
            var result = await validator.ValidateAsync(updateDto);

            if (!result.IsValid)
                return result;

            // Validaciones parciales
            if (updateDto.EmployeeId.HasValue)
            {
                var employeeValidation = await ValidateEmployeeExistsAsync(updateDto.EmployeeId.Value);
                if (!employeeValidation.IsValid)
                    result.Errors.AddRange(employeeValidation.Errors);
            }

            if (updateDto.ProjectId.HasValue)
            {
                var projectValidation = await ValidateProjectExistsAsync(updateDto.ProjectId.Value);
                if (!projectValidation.IsValid)
                    result.Errors.AddRange(projectValidation.Errors);
            }

            if (updateDto.CheckOut.HasValue)
            {
                var checkInOutValidation = await ValidateCheckInOutLogicAsync(updateDto.EmployeeId ?? 0, updateDto.CheckOut.Value);
                if (!checkInOutValidation.IsValid)
                    result.Errors.AddRange(checkInOutValidation.Errors);
            }

            return result;
        }

        public async Task<ValidationResult> ValidateEmployeeExistsAsync(int employeeId)
        {
            var result = new ValidationResult();

            if (employeeId <= 0)
            {
                result.Errors.Add(new ValidationFailure("EmployeeId", "El ID del empleado debe ser mayor que 0"));
                return result;
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                result.Errors.Add(new ValidationFailure("EmployeeId", $"No existe un empleado con ID {employeeId}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateProjectExistsAsync(int projectId)
        {
            var result = new ValidationResult();

            if (projectId <= 0)
            {
                result.Errors.Add(new ValidationFailure("ProjectId", "El ID del proyecto debe ser mayor que 0"));
                return result;
            }

            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
            {
                result.Errors.Add(new ValidationFailure("ProjectId", $"No existe un proyecto con ID {projectId}"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateCheckInOutLogicAsync(int employeeId, DateTime? checkOut)
        {
            var result = new ValidationResult();

            if (checkOut.HasValue)
            {
                var hasOpenCheckIn = await _assistanceRepository.HasOpenCheckInAsync(employeeId);
                if (hasOpenCheckIn)
                {
                    result.Errors.Add(new ValidationFailure("CheckOut", "El empleado ya tiene un CheckIn abierto. Debe cerrar la sesión anterior primero."));
                }
            }

            return result;
        }
    }
    public class CreateAssistanceValidator : AbstractValidator<CreateAssistanceDataTransferObject>
    {
        public CreateAssistanceValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0)
                .WithMessage("El ID del empleado debe ser mayor que 0");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0)
                .WithMessage("El ID del proyecto debe ser mayor que 0");

            RuleFor(x => x.CheckIn)
                .NotEmpty()
                .WithMessage("La fecha y hora de entrada es requerida")
                .Must(checkIn => checkIn <= DateTime.UtcNow.AddHours(1))
                .WithMessage("La fecha de entrada no puede ser más de 1 hora en el futuro");

            RuleFor(x => x.CheckOut)
                .Must((assistance, checkOut) => !checkOut.HasValue || checkOut.Value >= assistance.CheckIn)
                .WithMessage("La fecha de salida debe ser posterior o igual a la fecha de entrada");

            RuleFor(x => x.TotalHours)
                .Must(totalHours => !totalHours.HasValue || totalHours.Value >= 0)
                .WithMessage("Las horas totales no pueden ser negativas");

            RuleFor(x => x.RegisterType)
                .NotEmpty()
                .WithMessage("El tipo de registro es requerido")
                .Must(registerType => new[] { "CheckIn", "CheckOut", "Manual" }.Contains(registerType))
                .WithMessage("El tipo de registro debe ser: CheckIn, CheckOut o Manual");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder los 500 caracteres");
        }
    }

    public class UpdateAssistanceValidator : AbstractValidator<UpdateAssistanceDataTransferObject>
    {
        public UpdateAssistanceValidator()
        {
            RuleFor(x => x.EmployeeId)
                .Must(employeeId => !employeeId.HasValue || employeeId.Value > 0)
                .WithMessage("El ID del empleado debe ser mayor que 0");

            RuleFor(x => x.ProjectId)
                .Must(projectId => !projectId.HasValue || projectId.Value > 0)
                .WithMessage("El ID del proyecto debe ser mayor que 0");

            RuleFor(x => x.CheckIn)
                .Must(checkIn => !checkIn.HasValue || checkIn.Value <= DateTime.UtcNow.AddHours(1))
                .WithMessage("La fecha de entrada no puede ser más de 1 hora en el futuro");

            RuleFor(x => x.CheckOut)
                .Must((assistance, checkOut) => !checkOut.HasValue || !assistance.CheckIn.HasValue || checkOut.Value >= assistance.CheckIn.Value)
                .WithMessage("La fecha de salida debe ser posterior o igual a la fecha de entrada");

            RuleFor(x => x.TotalHours)
                .Must(totalHours => !totalHours.HasValue || totalHours.Value >= 0)
                .WithMessage("Las horas totales no pueden ser negativas");

            RuleFor(x => x.RegisterType)
                .Must(registerType => string.IsNullOrEmpty(registerType) || new[] { "CheckIn", "CheckOut", "Manual" }.Contains(registerType))
                .WithMessage("El tipo de registro debe ser: CheckIn, CheckOut o Manual");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder los 500 caracteres");
        }
    }
}
