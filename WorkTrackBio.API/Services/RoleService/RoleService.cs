using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Role;
using WorkTrackBio.API.Repositories.RoleRepository;

namespace WorkTrackBio.API.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<RoleDataTransferObject>>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<RoleDataTransferObject>>(roles);
            return ApiResponse<IEnumerable<RoleDataTransferObject>>.SuccessResponse(result, "Roles obtenidos exitosamente");
        }

        public async Task<ApiResponse<RoleDataTransferObject>> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse($"No se encontró un rol con ID {id}", 404);

            var result = _mapper.Map<RoleDataTransferObject>(role);
            return ApiResponse<RoleDataTransferObject>.SuccessResponse(result, "Rol obtenido exitosamente");
        }

        public async Task<ApiResponse<RoleDataTransferObject>> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return ApiResponse<RoleDataTransferObject>.ErrorResponse("El nombre del rol no puede estar vacío", 400);

            var role = await _roleRepository.GetByNameAsync(roleName);

            if (role == null)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse($"No se encontró un rol con el nombre '{roleName}'", 404);

            var result = _mapper.Map<RoleDataTransferObject>(role);
            return ApiResponse<RoleDataTransferObject>.SuccessResponse(result, "Rol obtenido exitosamente");
        }

        public async Task<ApiResponse<bool>> RoleExistsAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var role = await _roleRepository.GetByIdAsync(id);
            var exists = role != null;
            return ApiResponse<bool>.SuccessResponse(exists, exists ? "El rol existe" : "El rol no existe");
        }

        public async Task<ApiResponse<bool>> RoleNameExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return ApiResponse<bool>.ErrorResponse("El nombre del rol no puede estar vacío", 400);

            var exists = await _roleRepository.ExistsByNameAsync(roleName);
            return ApiResponse<bool>.SuccessResponse(exists, exists ? $"Ya existe un rol con el nombre '{roleName}'" : $"No existe un rol con el nombre '{roleName}'");
        }

        public async Task<ApiResponse<RoleDataTransferObject>> CreateRoleAsync(CreateRoleDataTransferObject createDto)
        {
            if (createDto == null)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse("Los datos del rol no pueden estar vacíos", 400);

            var existingRoles = await _roleRepository.GetAllAsync();
            if (existingRoles.Any(r => string.Equals(r.RoleName, createDto.RoleName, StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<RoleDataTransferObject>.ErrorResponse($"Ya existe un rol con el nombre '{createDto.RoleName}'", 409);

            var role = _mapper.Map<WorkTrackBio.API.Data.Models.Role>(createDto);
            var createdRole = await _roleRepository.CreateAsync(role);
            var result = _mapper.Map<RoleDataTransferObject>(createdRole);

            return ApiResponse<RoleDataTransferObject>.SuccessResponse(result, "Rol creado exitosamente", 201);
        }

        public async Task<ApiResponse<RoleDataTransferObject>> UpdateRoleAsync(UpdateRoleDataTransferObject updateDto)
        {
            if (updateDto == null)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse("Los datos del rol no pueden estar vacíos", 400);

            if (updateDto.Id <= 0)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var existingRole = await _roleRepository.GetByIdAsync(updateDto.Id);
            if (existingRole == null)
                return ApiResponse<RoleDataTransferObject>.ErrorResponse($"No se encontró un rol con ID {updateDto.Id}", 404);

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(updateDto.RoleName))
            {
                var allRoles = await _roleRepository.GetAllAsync();
                if (allRoles.Any(r => string.Equals(r.RoleName, updateDto.RoleName, StringComparison.OrdinalIgnoreCase) && r.Id != updateDto.Id))
                    return ApiResponse<RoleDataTransferObject>.ErrorResponse($"Ya existe un rol con el nombre '{updateDto.RoleName}'", 409);

                if (!string.Equals(existingRole.RoleName, updateDto.RoleName, StringComparison.OrdinalIgnoreCase))
                {
                    existingRole.RoleName = updateDto.RoleName;
                    hasChanges = true;
                }
            }

            if (updateDto.Description != null)
            {
                if (!string.Equals(existingRole.Description ?? "", updateDto.Description))
                {
                    existingRole.Description = updateDto.Description;
                    hasChanges = true;
                }
            }

            if (!hasChanges)
            {
                var unchanged = _mapper.Map<RoleDataTransferObject>(existingRole);
                return ApiResponse<RoleDataTransferObject>.SuccessResponse(unchanged, "No se detectaron cambios");
            }

            var updatedRole = await _roleRepository.UpdateAsync(existingRole);
            var result = _mapper.Map<RoleDataTransferObject>(updatedRole);
            return ApiResponse<RoleDataTransferObject>.SuccessResponse(result, "Rol actualizado exitosamente");
        }

        public async Task<ApiResponse<bool>> DeleteRoleAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<bool>.ErrorResponse($"No se encontró un rol con ID {id}", 404);

            var isInUse = await _roleRepository.HasDependenciesAsync(id);
            if (isInUse)
                return ApiResponse<bool>.ErrorResponse($"No se puede eliminar el rol '{role.RoleName}' porque está siendo usado por usuarios del sistema", 409);

            await _roleRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Rol eliminado exitosamente");
        }
    }
}