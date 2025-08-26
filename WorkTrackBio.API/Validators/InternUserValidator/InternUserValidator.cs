using FluentValidation;
using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.InternUser;
using WorkTrackBio.API.Repositories.InternUserRepository;
using WorkTrackBio.API.Repositories.RoleRepository;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;
using WorkTrackBio.API.Validators.DocumentValidator;

namespace WorkTrackBio.API.Validators.InternUserValidator
{
    public class InternUserValidator : IInternUserValidator
    {
        private readonly IInternUserRepository _internUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IDocumentValidator _documentValidator;
        private readonly CreateInternUserValidator _createValidator;
        private readonly UpdateInternUserValidator _updateValidator;

        public InternUserValidator(
            IInternUserRepository internUserRepository,
            IRoleRepository roleRepository,
            IStateRepository stateRepository,
            IDocumentTypeRepository documentTypeRepository,
            IDocumentValidator documentValidator)
        {
            _internUserRepository = internUserRepository ?? throw new ArgumentNullException(nameof(internUserRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
            _documentValidator = documentValidator ?? throw new ArgumentNullException(nameof(documentValidator));
            
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

            // Validar documento si se proporciona
            if (!string.IsNullOrWhiteSpace(createDto.DocumentNumber) || createDto.DocumentTypeId.HasValue)
            {
                var documentValidation = await ValidateDocumentAsync(createDto.DocumentNumber, createDto.DocumentTypeId, createDto.DocumentExpire);
                if (!documentValidation.IsValid)
                    return documentValidation;
            }

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
                result.Errors.Add(new ValidationFailure("Id", $"No existe un usuario con ese ID {updateDto.Id}"));
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

            // Validar documento si se está actualizando
            if (!string.IsNullOrWhiteSpace(updateDto.DocumentNumber) || updateDto.DocumentTypeId.HasValue)
            {
                var documentValidation = await ValidateDocumentAsync(updateDto.DocumentNumber, updateDto.DocumentTypeId, updateDto.DocumentExpire);
                if (!documentValidation.IsValid)
                    return documentValidation;
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
                result.Errors.Add(new ValidationFailure("Email", "Correo Electrónico requerido"));
                return result;
            }

            // Validar formato de email
            if (!IsValidEmailFormat(email))
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", "El formato del correo electrónico no es válido"));
                return result;
            }

            // Validar longitud máxima
            if (email.Length > 100)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", "El correo electrónico no puede tener más de 100 caracteres"));
                return result;
            }

            // Validar unicidad
            var existingUser = await _internUserRepository.GetByEmailAsync(email);
            if (existingUser != null && (!excludeUserId.HasValue || existingUser.Id != excludeUserId.Value))
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("Email", $"Correo electrónico en uso '{email}'"));
                return result;
            }

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateRoleExistsAsync(int roleId)
        {
            if (roleId <= 0)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("RolId", "Se debe de asignar un rol al usuario"));
                return result;
            }

            var roleExists = await _roleRepository.GetByIdAsync(roleId);
            if (roleExists == null)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("RolId", $"No existe el rol asignado  {roleId}"));
                return result;
            }

            return new ValidationResult();
        }

        public async Task<ValidationResult> ValidateDocumentAsync(string? documentNumber, int? documentTypeId, DateOnly? documentExpire = null)
        {
            var result = new ValidationResult();

            // Validar que si se proporciona documento, también se proporcione el tipo
            if (!string.IsNullOrWhiteSpace(documentNumber) && !documentTypeId.HasValue)
            {
                result.Errors.Add(new ValidationFailure("DocumentTypeId", "Si se proporciona un número de documento, también debe especificarse el tipo de documento"));
            }

            if (documentTypeId.HasValue && string.IsNullOrWhiteSpace(documentNumber))
            {
                result.Errors.Add(new ValidationFailure("DocumentNumber", "Si se especifica un tipo de documento, también debe proporcionarse el número de documento"));
            }

            // Validar que el tipo de documento exista
            if (documentTypeId.HasValue)
            {
                var documentTypeExists = await _documentTypeRepository.GetByIdAsync(documentTypeId.Value);
                if (documentTypeExists == null)
                {
                    result.Errors.Add(new ValidationFailure("DocumentTypeId", $"No existe un tipo de documento con ID {documentTypeId.Value}"));
                }
            }

            // Validar formato del número de documento según su tipo usando DocumentValidator
            if (!string.IsNullOrWhiteSpace(documentNumber) && documentTypeId.HasValue)
            {
                var documentValidation = await _documentValidator.ValidateDocumentAsync(documentNumber, documentTypeId.Value);
                if (!documentValidation.IsValid)
                {
                    result.Errors.Add(new ValidationFailure("DocumentNumber", documentValidation.ErrorMessage ?? "Formato de documento inválido"));
                }
            }

            // Validar fecha de expiración
            if (documentExpire.HasValue)
            {
                var expirationValidation = ValidateDocumentExpiration(documentExpire.Value);
                if (!expirationValidation.IsValid)
                {
                    result.Errors.AddRange(expirationValidation.Errors);
                }
            }

            return result;
        }

        public ValidationResult ValidateDocumentExpiration(DateOnly documentExpire)
        {
            var result = new ValidationResult();
            var today = DateOnly.FromDateTime(DateTime.Today);

            if (documentExpire < today)
            {
                result.Errors.Add(new ValidationFailure("DocumentExpire", "La fecha de expiración del documento no puede ser anterior a hoy"));
            }

            return result;
        }

        public async Task<ValidationResult> ValidateStateExistsAsync(int stateId)
        {
            if (stateId <= 0)
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("StateId", "Se debe asignar el estado del usuario"));
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
                    .NotEmpty().WithMessage("El correo electrónico es obligatorio")
                    .MaximumLength(100).WithMessage("El email no puede tener más de 100 caracteres");

                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("El nombre es obligatorio")
                    .MaximumLength(50).WithMessage("El nombre excede el maximo permitido")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios");

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("El apellido es obligatorio")
                    .MaximumLength(100).WithMessage("El apellido excede el maximo permitido")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El apellido solo puede contener letras y espacios");

                RuleFor(x => x.RolId)
                    .GreaterThan(0).WithMessage("El Rol del usuario es obligatorio");

                RuleFor(x => x.StateId)
                    .GreaterThan(0).WithMessage("El estado del usuario es obligatorio");

                // Validaciones de documento (opcionales)
                RuleFor(x => x.DocumentNumber)
                    .MaximumLength(50).WithMessage("El número de documento no puede exceder 50 caracteres")
                    .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

                RuleFor(x => x.DocumentTypeId)
                    .GreaterThan(0).WithMessage("El ID del tipo de documento debe ser mayor que 0")
                    .When(x => x.DocumentTypeId.HasValue);
            }
        }

        private class UpdateInternUserValidator : AbstractValidator<UpdateInternUserDataTransferObject>
        {
            public UpdateInternUserValidator()
            {
                RuleFor(x => x.Id)
                    .GreaterThan(0).WithMessage("El ID debe ser mayor que 0");

                RuleFor(x => x.Email)
                    .MaximumLength(100).WithMessage("La longitud del correo electronico excede el maximo permitido")
                    .When(x => x.Email != null);

                RuleFor(x => x.FirstName)
                    .MaximumLength(50).WithMessage("La longitud del nombre excede el maximo permitido")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El nombre solo puede contener letras y espacios")
                    .When(x => x.FirstName != null);

                RuleFor(x => x.LastName)
                    .MaximumLength(100).WithMessage("La longitud del apellido excede el maximo permitido")
                    .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("El apellido solo puede contener letras y espacios")
                    .When(x => x.LastName != null);

                RuleFor(x => x.RolId)
                    .GreaterThan(0).WithMessage("Rol del usuario requerido")
                    .When(x => x.RolId.HasValue);

                RuleFor(x => x.StateId)
                    .GreaterThan(0).WithMessage("Estado del usuario requerido")
                    .When(x => x.StateId.HasValue);

                // Validaciones de documento (opcionales)
                RuleFor(x => x.DocumentNumber)
                    .MaximumLength(50).WithMessage("El número de documento no puede exceder 50 caracteres")
                    .When(x => !string.IsNullOrWhiteSpace(x.DocumentNumber));

                RuleFor(x => x.DocumentTypeId)
                    .GreaterThan(0).WithMessage("El ID del tipo de documento debe ser mayor que 0")
                    .When(x => x.DocumentTypeId.HasValue);
            }
        }
    }
}

