using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.InternUser;
using WorkTrackBio.API.Repositories.InternUserRepository;
using WorkTrackBio.API.Repositories.RoleRepository;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Validators.InternUserValidator
{
    public class InternUserValidator : IInternUserValidator
    {
        private readonly IInternUserRepository _internUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStateRepository _stateRepository;
        private readonly CreateInternUserValidator _createValidator;
        private readonly UpdateInternUserValidator _updateValidator;

        public InternUserValidator(
            IInternUserRepository internUserRepository,
            IRoleRepository roleRepository,
            IStateRepository stateRepository)
        {
            _internUserRepository = internUserRepository ?? throw new ArgumentNullException(nameof(internUserRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            
            _createValidator = new CreateInternUserValidator();
            _updateValidator = new UpdateInternUserValidator();
        }

        public async Task<ValidationResult> ValidateCreateAsync(CreateInternUserDataTransferObject createDto)
        {
            if (createDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("CreateDto", "El DTO de creación no puede ser nulo"));
                return result;
            }

            // Validar reglas básicas del DTO
            var basicValidation = await _createValidator.ValidateAsync(createDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            // Validar que el rol exista
            var roleValidation = await ValidateRoleExistsAsync(createDto.RolId);
            if (!roleValidation.IsValid)
                return roleValidation;

            // Validar que el estado exista
            var stateValidation = await ValidateStateExistsAsync(createDto.StateId);
            if (!stateValidation.IsValid)
                return stateValidation;

            // Validar que el email sea único
            var emailValidation = await ValidateEmailAsync(createDto.Email);
            if (!emailValidation.IsValid)
                return emailValidation;

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateUpdateAsync(UpdateInternUserDataTransferObject updateDto)
        {
            if (updateDto == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("UpdateDto", "El DTO de actualización no puede ser nulo"));
                return result;
            }

            // Validar reglas básicas del DTO
            var basicValidation = await _updateValidator.ValidateAsync(updateDto);
            if (!basicValidation.IsValid)
                return basicValidation;

            // Verificar que el usuario exista
            var userExists = await _internUserRepository.ExistsByIdAsync(updateDto.Id);
            if (!userExists)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Id", $"No existe un usuario interno con ID {updateDto.Id}"));
                return result;
            }

            // Validar rol si se está actualizando
            if (updateDto.RolId.HasValue)
            {
                var roleValidation = await ValidateRoleExistsAsync(updateDto.RolId.Value);
                if (!roleValidation.IsValid)
                    return roleValidation;
            }

            // Validar estado si se está actualizando
            if (updateDto.StateId.HasValue)
            {
                var stateValidation = await ValidateStateExistsAsync(updateDto.StateId.Value);
                if (!stateValidation.IsValid)
                    return stateValidation;
            }

            // Validar email si se está actualizando
            if (updateDto.Email != null)
            {
                var emailValidation = await ValidateEmailAsync(updateDto.Email, updateDto.Id);
                if (!emailValidation.IsValid)
                    return emailValidation;
            }

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateEmailAsync(string email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", "El email no puede estar vacío"));
                return result;
            }

            // Validar formato de email
            if (!IsValidEmailFormat(email))
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", "El formato del email no es válido"));
                return result;
            }

            // Validar longitud máxima
            if (email.Length > 100)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", "El email no puede tener más de 100 caracteres"));
                return result;
            }

            // Validar unicidad
            var existingUser = await _internUserRepository.GetByEmailAsync(email);
            if (existingUser != null && (!excludeUserId.HasValue || existingUser.Id != excludeUserId.Value))
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", $"Ya existe un usuario con el email '{email}'"));
                return result;
            }

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateRoleExistsAsync(int roleId)
        {
            if (roleId <= 0)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("RolId", "El ID del rol debe ser mayor que 0"));
                return result;
            }

            var roleExists = await _roleRepository.GetByIdAsync(roleId);
            if (roleExists == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("RolId", $"No existe un rol con ID {roleId}"));
                return result;
            }

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateStateExistsAsync(int stateId)
        {
            if (stateId <= 0)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("StateId", "El ID del estado debe ser mayor que 0"));
                return result;
            }

            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("StateId", $"No existe un estado con ID {stateId}"));
                return result;
            }

            return new ValidationResult();
        }

        private bool IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Validadores internos para los DTOs
        private class CreateInternUserValidator : AbstractValidator<CreateInternUserDataTransferObject>
        {
            public CreateInternUserValidator()
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("El email es obligatorio")
                    .MaximumLength(100).WithMessage("El email no puede tener más de 100 caracteres");

                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("El nombre es obligatorio")
                    .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("El apellido es obligatorio")
                    .MaximumLength(100).WithMessage("El apellido no puede tener más de 100 caracteres")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El apellido solo puede contener letras y espacios");

                RuleFor(x => x.RolId)
                    .GreaterThan(0).WithMessage("El ID del rol debe ser mayor que 0");

                RuleFor(x => x.StateId)
                    .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0");
            }
        }

        private class UpdateInternUserValidator : AbstractValidator<UpdateInternUserDataTransferObject>
        {
            public UpdateInternUserValidator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0).WithMessage("El ID debe ser mayor que 0");

                RuleFor(x => x.Email)
                    .MaximumLength(100).WithMessage("El email no puede tener más de 100 caracteres")
                    .When(x => x.Email != null);

                RuleFor(x => x.FirstName)
                    .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios")
                    .When(x => x.FirstName != null);

                RuleFor(x => x.LastName)
                    .MaximumLength(100).WithMessage("El apellido no puede tener más de 100 caracteres")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El apellido solo puede contener letras y espacios")
                    .When(x => x.LastName != null);

                RuleFor(x => x.RolId)
                    .GreaterThan(0).WithMessage("El ID del rol debe ser mayor que 0")
                    .When(x => x.RolId.HasValue);

                RuleFor(x => x.StateId)
                    .GreaterThan(0).WithMessage("El ID del estado debe ser mayor que 0")
                    .When(x => x.StateId.HasValue);
            }
        }
    }
}
