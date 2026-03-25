using System.Reflection.Metadata.Ecma335;
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

        public async Task<ApiResponse<InternUserDataTransferObject>> GetInternUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("Correo Electronico Obligatorio", 400);

            var internUser = await _internUserRepository.GetByEmailAsync(email);
            
            if (internUser == null)
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No se encontró un usuario con el correo electronico '{email}'", 404);

            var result = _mapper.Map<InternUserDataTransferObject>(internUser);
            return ApiResponse<InternUserDataTransferObject>.SuccessResponse(result, "Usuario obtenido exitosamente");
        }

        public async Task<ApiResponse<InternUserDataTransferObject>> GetByDocumentAsync(string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El número de documento no puede estar vacío", 400);

            var internUser = await _internUserRepository.GetByDocumentAsync(documentNumber);

            if (internUser == null)
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No se encontró un usuario con el número de documento '{documentNumber}'", 404);

            var result = _mapper.Map<InternUserDataTransferObject>(internUser);
            return ApiResponse<InternUserDataTransferObject>.SuccessResponse(result, "Usuario obtenido exitosamente");
        }

        public async Task<ApiResponse<IEnumerable<InternUserDataTransferObject>>> GetInternUsersByRoleAsync(int roleId)
        {
            if (roleId <= 0)
                return ApiResponse<IEnumerable<InternUserDataTransferObject>>.ErrorResponse($"El ID debe ser mayor que 0", 400);

            var internUsers = await _internUserRepository.GetByRoleAsync(roleId);
            var result = _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);

            return ApiResponse<IEnumerable<InternUserDataTransferObject>>.SuccessResponse(result, $"Se encontraron {result.Count()} usuarios con el ID provisionado");
        }

        public async Task<ApiResponse<IEnumerable<InternUserDataTransferObject>>> GetInternUsersByStateAsync(int stateId)
        {
            if (stateId <= 0)
                return ApiResponse<IEnumerable<InternUserDataTransferObject>>.ErrorResponse("El ID del estado debe ser mayor que 0", 400);
            
            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                return ApiResponse <IEnumerable<InternUserDataTransferObject>>.ErrorResponse("El estado provisionado no existe", 400);

            var internUsers = await _internUserRepository.GetByStateAsync(stateId);
            var result = _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);
            return ApiResponse<IEnumerable<InternUserDataTransferObject>>.SuccessResponse(result, $"Se encontraron {result.Count()} usuarios");
        }

        public async Task<ApiResponse<InternUserDataTransferObject>> CreateInternUserAsync(CreateInternUserDataTransferObject createDto)
        {
            var validationResult = await _internUserValidator.ValidateCreateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("Datos invalidos, verifique antes de continuar", 400);
            }

            // Verificar que el rol exista
            var roleExists = await _roleRepository.GetByIdAsync(createDto.RolId);
            if (roleExists == null)
            {
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El rol especificado no existe", 400);
            }

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(createDto.StateId);
            if (stateExists == null)
            {
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El estado especificado no existe", 400);
            }

            // Verificar que el email sea único
            var existingUser = await _internUserRepository.GetByEmailAsync(createDto.Email);
            if (existingUser != null)
            {
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El correo ya existe", 400);
            }

            var internUser = _mapper.Map<InternUser>(createDto);
            var createdInternUser = await _internUserRepository.CreateAsync(internUser);
            var result = _mapper.Map<InternUserDataTransferObject>(createdInternUser);

            return ApiResponse<InternUserDataTransferObject>.SuccessResponse(result,"Usuario creado sastifactoriamente", 201);
        }

        public async Task<ApiResponse<InternUserDataTransferObject>> UpdateInternUserAsync(UpdateInternUserDataTransferObject updateDto)
        {
            var validationResult = await _internUserValidator.ValidateUpdateAsync(updateDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse("Datos invalidos, vuelva a verificar", 400);
            }

            var existingUser = await _internUserRepository.GetByIdAsync(updateDto.Id);
            if (existingUser == null)
            {
                return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No existe usuario con ID {updateDto.Id} en el sistema ", 404);
            }

            if (updateDto.RolId.HasValue)
            {
                var roleExists = await _roleRepository.GetByIdAsync(updateDto.RolId.Value);
                if (roleExists == null)
                {
                    return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No existe el role ID {updateDto.RolId}", 404);
                }
            }

            if (updateDto.StateId.HasValue)
            {
                var stateExists = await _stateRepository.GetByIdAsync(updateDto.StateId.Value);
                if (stateExists == null)
                {
                    return ApiResponse<InternUserDataTransferObject>.ErrorResponse($"No existe el estado {updateDto.StateId}", 404);
                }
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != existingUser.Email)
            {
                var userWithEmail = await _internUserRepository.GetByEmailAsync(updateDto.Email);
                if (userWithEmail != null)
                {
                    return ApiResponse<InternUserDataTransferObject>.ErrorResponse("El correo electronico ya existe", 400);
                }
            }

            var internUser = _mapper.Map<InternUser>(updateDto);
            var updatedInternUser = await _internUserRepository.UpdateAsync(internUser);
            var result = _mapper.Map<InternUserDataTransferObject>(updatedInternUser);

            return ApiResponse<InternUserDataTransferObject>.SuccessResponse(result, "Usuario actualizado correctamente");
        }

        public async Task<ApiResponse<bool>> DeleteInternUserAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var existingUser = await _internUserRepository.GetByIdAsync(id);
            if (existingUser == null)
                return ApiResponse<bool>.ErrorResponse("No se encontro el usuario solicitado", 404);

            await _internUserRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Usuario eliminado sastifactoriamente");
        }
    }
}
