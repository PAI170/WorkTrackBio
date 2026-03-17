using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.InternUser;
using WorkTrackBio.API.Repositories.InternUserRepository;
using WorkTrackBio.API.Repositories.RoleRepository;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Validators.InternUserValidator;

namespace WorkTrackBio.API.Services.InternUserService
{
    public class InternUserService : IInternUserService
    {
        private readonly IInternUserRepository _internUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IInternUserValidator _internUserValidator;
        private readonly IMapper _mapper;

        public InternUserService(
            IInternUserRepository internUserRepository,
            IRoleRepository roleRepository,
            IStateRepository stateRepository,
            IInternUserValidator internUserValidator,
            IMapper mapper)
        {
            _internUserRepository = internUserRepository ?? throw new ArgumentNullException(nameof(internUserRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _internUserValidator = internUserValidator ?? throw new ArgumentNullException(nameof(internUserValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<InternUserDataTransferObject>>> GetAllInternUsersAsync()
        {
            var internUsers = await _internUserRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);

            return ApiResponse<IEnumerable<InternUserDataTransferObject>>.SuccessResponse(result, $"Se encontraron {result.Count()} usuarios");
        }

        public async Task<ApiResponse<InternUserDataTransferObject>> GetInternUserByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var internUser = await _internUserRepository.GetByIdAsync(id);

            if (internUser == null)
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No se encontro el usuario con el ID {id}", 404);

            var result = _mapper.Map<InternUserDataTransferObject>(internUser);
            return ApiResponse<InternUserDataTransferObject>.SuccessResponse(result, "Usuarios obtenidos exitosamente");
        }

        public async Task<InternUserDataTransferObject?> GetInternUserByEmailAsync(string email)
        {
            var internUser = await _internUserRepository.GetByEmailAsync(email);
            return _mapper.Map<InternUserDataTransferObject>(internUser);
        }

        public async Task<ApiResponse<InternUserDataTransferObject>> GetInternUserByDocumentAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El número de documento no puede estar vacío", 400);

            var internUser = await _internUserRepository.GetInternUserByDocumentAsync(documentNumber);

            if (internUser == null)
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No se encontró un usuario con el número de documento '{documentNumber}'", 404);

            var result = _mapper.Map<InternUserDataTransferObject>(internUser);
            return ApiResponse<InternUserDataTransferObject>.SuccessResponse(result, "Usuario obtenido exitosamente");
        }

        public async Task<InternUserDataTransferObject?> GetInternUserByIdentifierAsync(string identifier)
        {
            var internUser = await _internUserRepository.GetByIdentifierAsync(identifier);
            return _mapper.Map<InternUserDataTransferObject>(internUser);
        }

        public async Task<IEnumerable<InternUserDataTransferObject>> GetInternUsersByRoleAsync(int roleId)
        {
            var internUsers = await _internUserRepository.GetByRoleAsync(roleId);
            return _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);
        }

        public async Task<IEnumerable<InternUserDataTransferObject>> GetInternUsersByStateAsync(int stateId)
        {
            var internUsers = await _internUserRepository.GetByStateAsync(stateId);
            return _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);
        }

        public async Task<InternUserDataTransferObject> CreateInternUserAsync(CreateInternUserDataTransferObject createDto)
        {
            // Validar DTO antes de crear
            var validationResult = await _internUserValidator.ValidateCreateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Validación fallida: {errors}");
            }

            // Verificar que el rol exista
            var roleExists = await _roleRepository.GetByIdAsync(createDto.RolId);
            if (roleExists == null)
            {
                throw new ArgumentException($"No existe un rol con ID {createDto.RolId}");
            }

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(createDto.StateId);
            if (stateExists == null)
            {
                throw new ArgumentException($"No existe un estado con ID {createDto.StateId}");
            }

            // Verificar que el email sea único
            var existingUser = await _internUserRepository.GetByEmailAsync(createDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"Ya existe un usuario con el email '{createDto.Email}'");
            }

            var internUser = _mapper.Map<InternUser>(createDto);
            var createdInternUser = await _internUserRepository.CreateAsync(internUser);
            
            return _mapper.Map<InternUserDataTransferObject>(createdInternUser);
        }

        public async Task<InternUserDataTransferObject?> UpdateInternUserAsync(UpdateInternUserDataTransferObject updateDto)
        {
            // Validar DTO antes de actualizar
            var validationResult = await _internUserValidator.ValidateUpdateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Validación fallida: {errors}");
            }

            // Verificar que el usuario exista
            var existingUser = await _internUserRepository.GetByIdAsync(updateDto.Id);
            if (existingUser == null)
            {
                throw new ArgumentException($"No existe un usuario con ID {updateDto.Id}");
            }

            // Verificar rol si se está actualizando
            if (updateDto.RolId.HasValue)
            {
                var roleExists = await _roleRepository.GetByIdAsync(updateDto.RolId.Value);
                if (roleExists == null)
                {
                    throw new ArgumentException($"No existe un rol con ID {updateDto.RolId.Value}");
                }
            }

            // Verificar estado si se está actualizando
            if (updateDto.StateId.HasValue)
            {
                var stateExists = await _stateRepository.GetByIdAsync(updateDto.StateId.Value);
                if (stateExists == null)
                {
                    throw new ArgumentException($"No existe un estado con ID {updateDto.StateId.Value}");
                }
            }

            // Verificar email único si se está actualizando
            if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != existingUser.Email)
            {
                var userWithEmail = await _internUserRepository.GetByEmailAsync(updateDto.Email);
                if (userWithEmail != null)
                {
                    throw new InvalidOperationException($"Ya existe un usuario con el email '{updateDto.Email}'");
                }
            }

            var internUser = _mapper.Map<InternUser>(updateDto);
            var updatedInternUser = await _internUserRepository.UpdateAsync(internUser);
            
            return _mapper.Map<InternUserDataTransferObject>(updatedInternUser);
        }

        public async Task<bool> DeleteInternUserAsync(int id)
        {
            // Verificar que el usuario exista
            var existingUser = await _internUserRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                return false;
            }

            // Aquí se podrían agregar validaciones adicionales antes de eliminar
            // Por ejemplo, verificar que no tenga dependencias en otras tablas

            return await _internUserRepository.DeleteAsync(id);
        }

        public async Task<bool> InternUserExistsAsync(int id)
        {
            return await _internUserRepository.ExistsByIdAsync(id);
        }

        public async Task<bool> InternUserEmailExistsAsync(string email)
        {
            var user = await _internUserRepository.GetByEmailAsync(email);
            return user != null;
        }
    }
}
