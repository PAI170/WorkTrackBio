using AutoMapper;
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

        public async Task<IEnumerable<RoleDataTransferObject>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDataTransferObject>>(roles);
        }

        public async Task<RoleDataTransferObject?> GetRoleByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var role = await _roleRepository.GetByIdAsync(id);
            return _mapper.Map<RoleDataTransferObject>(role);
        }

        public async Task<RoleDataTransferObject?> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("El nombre del rol no puede estar vacío", nameof(roleName));

            var role = await _roleRepository.GetByNameAsync(roleName);
            return _mapper.Map<RoleDataTransferObject>(role);
        }

        public async Task<bool> RoleExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            var role = await _roleRepository.GetByIdAsync(id);
            return role != null;
        }

        public async Task<bool> RoleNameExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            return await _roleRepository.ExistsByNameAsync(roleName);
        }

        public async Task<RoleDataTransferObject> CreateRoleAsync(CreateRoleDataTransferObject createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Verificar si ya existe un rol con el mismo nombre (case-insensitive)
            var existingRoles = await _roleRepository.GetAllAsync();
            if (existingRoles.Any(r => string.Equals(r.RoleName, createDto.RoleName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ya existe un rol con el nombre '{createDto.RoleName}' (ignorando mayúsculas/minúsculas)");

            // Mapear DTO a Model
            var role = _mapper.Map<WorkTrackBio.API.Data.Models.Role>(createDto);

            // Crear el rol
            var createdRole = await _roleRepository.CreateAsync(role);

            // Mapear de vuelta a DTO y retornar
            return _mapper.Map<RoleDataTransferObject>(createdRole);
        }

        public async Task<RoleDataTransferObject?> UpdateRoleAsync(UpdateRoleDataTransferObject updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(updateDto.Id));

            // Verificar si el rol existe
            var existingRole = await _roleRepository.GetByIdAsync(updateDto.Id);
            if (existingRole == null)
                return null;

            // Lógica de "Partial Update": Solo actualizar campos que realmente cambiaron
            bool hasChanges = false;

            // Actualizar RoleName solo si se proporcionó un nuevo valor
            if (!string.IsNullOrWhiteSpace(updateDto.RoleName))
            {
                // Verificar si el nuevo nombre ya existe en otro rol (case-insensitive)
                var allRoles = await _roleRepository.GetAllAsync();
                if (allRoles.Any(r => string.Equals(r.RoleName, updateDto.RoleName, StringComparison.OrdinalIgnoreCase) &&
                    r.Id != updateDto.Id))
                    throw new InvalidOperationException($"Ya existe un rol con el nombre '{updateDto.RoleName}' (ignorando mayúsculas/minúsculas)");

                // Solo actualizar si realmente cambió
                if (!string.Equals(existingRole.RoleName, updateDto.RoleName, StringComparison.OrdinalIgnoreCase))
                {
                    existingRole.RoleName = updateDto.RoleName;
                    hasChanges = true;
                }
            }

            // Actualizar Description solo si se proporcionó un nuevo valor
            if (updateDto.Description != null) // null significa "sin cambios"
            {
                if (!string.Equals(existingRole.Description ?? "", updateDto.Description))
                {
                    existingRole.Description = updateDto.Description;
                    hasChanges = true;
                }
            }

            // Si no hay cambios, retornar el rol existente sin modificar
            if (!hasChanges)
            {
                return _mapper.Map<RoleDataTransferObject>(existingRole);
            }

            // Guardar cambios solo si hubo modificaciones
            var updatedRole = await _roleRepository.UpdateAsync(existingRole);

            // Mapear de vuelta a DTO y retornar
            return _mapper.Map<RoleDataTransferObject>(updatedRole);
        }

        public async Task<bool> DeleteRoleAsync(int id)
        {
            if (id <= 0)
                return false;

            // Verificar si el rol existe
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return false;

            // Verificar si el rol está siendo usado por otras entidades
            // Esto evita errores de Foreign Key constraint
            var isRoleInUse = await IsRoleInUseAsync(id);
            if (isRoleInUse)
                throw new InvalidOperationException($"No se puede eliminar el rol '{role.RoleName}' porque está siendo usado por usuarios internos del sistema");

            return await _roleRepository.DeleteAsync(id);
        }

        private async Task<bool> IsRoleInUseAsync(int roleId)
        {
            // Verificar si hay entidades que dependen de este rol
            var hasDependencies = await _roleRepository.HasDependenciesAsync(roleId);
            return hasDependencies;
        }
    }
}
