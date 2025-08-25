using AutoMapper;
using WorkTrackBio.API.DataTransferObjects.InternUser;
using WorkTrackBio.API.Repositories.InternUserRepository;
using WorkTrackBio.API.Repositories.RoleRepository;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Services.InternUserService
{
    public class InternUserService : IInternUserService
    {
        private readonly IInternUserRepository _internUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public InternUserService(
            IInternUserRepository internUserRepository,
            IRoleRepository roleRepository,
            IStateRepository stateRepository,
            IMapper mapper)
        {
            _internUserRepository = internUserRepository ?? throw new ArgumentNullException(nameof(internUserRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<InternUserDataTransferObject>> GetAllInternUsersAsync()
        {
            var internUsers = await _internUserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);
        }

        public async Task<InternUserDataTransferObject?> GetInternUserByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var internUser = await _internUserRepository.GetByIdAsync(id);
            return _mapper.Map<InternUserDataTransferObject>(internUser);
        }

        public async Task<InternUserDataTransferObject?> GetInternUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email no puede estar vacío", nameof(email));

            var internUser = await _internUserRepository.GetByEmailAsync(email);
            return _mapper.Map<InternUserDataTransferObject>(internUser);
        }

        public async Task<IEnumerable<InternUserDataTransferObject>> GetInternUsersByRoleAsync(int roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("El ID del rol debe ser mayor que 0", nameof(roleId));

            // Verificar que el rol exista
            var roleExists = await _roleRepository.GetByIdAsync(roleId);
            if (roleExists == null)
                throw new ArgumentException($"No existe un rol con ID {roleId}", nameof(roleId));

            var internUsers = await _internUserRepository.GetByRoleAsync(roleId);
            return _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);
        }

        public async Task<IEnumerable<InternUserDataTransferObject>> GetInternUsersByStateAsync(int stateId)
        {
            if (stateId <= 0)
                throw new ArgumentException("El ID del estado debe ser mayor que 0", nameof(stateId));

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                throw new ArgumentException($"No existe un estado con ID {stateId}", nameof(stateId));

            var internUsers = await _internUserRepository.GetByStateAsync(stateId);
            return _mapper.Map<IEnumerable<InternUserDataTransferObject>>(internUsers);
        }

        public async Task<bool> InternUserExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            var internUser = await _internUserRepository.GetByIdAsync(id);
            return internUser != null;
        }

        public async Task<bool> InternUserEmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _internUserRepository.ExistsByEmailAsync(email);
        }

        public async Task<InternUserDataTransferObject> CreateInternUserAsync(CreateInternUserDataTransferObject createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Verificar que el rol exista
            var roleExists = await _roleRepository.GetByIdAsync(createDto.RolId);
            if (roleExists == null)
                throw new ArgumentException($"No existe un rol con ID {createDto.RolId}", nameof(createDto.RolId));

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(createDto.StateId);
            if (stateExists == null)
                throw new ArgumentException($"No existe un estado con ID {createDto.StateId}", nameof(createDto.StateId));

            // Verificar que no exista otro usuario con el mismo email
            var existingUser = await _internUserRepository.GetByEmailAsync(createDto.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"Ya existe un usuario con el email '{createDto.Email}'");

            var internUser = _mapper.Map<WorkTrackBio.API.Data.Models.InternUser>(createDto);
            
            var createdInternUser = await _internUserRepository.CreateAsync(internUser);
            return _mapper.Map<InternUserDataTransferObject>(createdInternUser);
        }

        public async Task<InternUserDataTransferObject?> UpdateInternUserAsync(UpdateInternUserDataTransferObject updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(updateDto.Id));

            var existingUser = await _internUserRepository.GetByIdAsync(updateDto.Id);
            if (existingUser == null)
                return null;

            bool hasChanges = false;

            // Crear un diccionario para rastrear qué campos fueron explícitamente enviados
            var sentFields = new HashSet<string>();
            var updateDtoType = typeof(UpdateInternUserDataTransferObject);
            foreach (var property in updateDtoType.GetProperties())
            {
                if (property.Name != "Id" && property.GetValue(updateDto) != null)
                {
                    sentFields.Add(property.Name);
                }
            }

            // Validar y actualizar email
            if (sentFields.Contains("Email"))
            {
                if (updateDto.Email == null)
                {
                    // Campo explícitamente enviado como null
                    existingUser.Email = string.Empty;
                    hasChanges = true;
                }
                else
                {
                    // Verificar que no exista otro usuario con el mismo email
                    var otherUser = await _internUserRepository.GetByEmailAsync(updateDto.Email);
                    if (otherUser != null && otherUser.Id != updateDto.Id)
                        throw new InvalidOperationException($"Ya existe otro usuario con el email '{updateDto.Email}'");

                    if (existingUser.Email != updateDto.Email)
                    {
                        existingUser.Email = updateDto.Email;
                        hasChanges = true;
                    }
                }
            }

            // Validar y actualizar nombres
            if (sentFields.Contains("FirstName") && updateDto.FirstName != null &&
                !string.Equals(existingUser.FirstName, updateDto.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                existingUser.FirstName = updateDto.FirstName;
                hasChanges = true;
            }

            if (sentFields.Contains("LastName") && updateDto.LastName != null &&
                !string.Equals(existingUser.LastName, updateDto.LastName, StringComparison.OrdinalIgnoreCase))
            {
                existingUser.LastName = updateDto.LastName;
                hasChanges = true;
            }

            // Validar y actualizar rol
            if (sentFields.Contains("RolId") && updateDto.RolId.HasValue)
            {
                var roleExists = await _roleRepository.GetByIdAsync(updateDto.RolId.Value);
                if (roleExists == null)
                    throw new ArgumentException($"No existe un rol con ID {updateDto.RolId.Value}", nameof(updateDto.RolId));

                if (existingUser.RolId != updateDto.RolId.Value)
                {
                    existingUser.RolId = updateDto.RolId.Value;
                    hasChanges = true;
                }
            }

            // Validar y actualizar estado
            if (sentFields.Contains("StateId") && updateDto.StateId.HasValue)
            {
                var stateExists = await _stateRepository.GetByIdAsync(updateDto.StateId.Value);
                if (stateExists == null)
                    throw new ArgumentException($"No existe un estado con ID {updateDto.StateId.Value}", nameof(updateDto.StateId));

                if (existingUser.StateId != updateDto.StateId.Value)
                {
                    existingUser.StateId = updateDto.StateId.Value;
                    hasChanges = true;
                }
            }

            if (!hasChanges)
            {
                return _mapper.Map<InternUserDataTransferObject>(existingUser);
            }

            var updatedInternUser = await _internUserRepository.UpdateAsync(existingUser);
            return _mapper.Map<InternUserDataTransferObject>(updatedInternUser);
        }

        public async Task<bool> DeleteInternUserAsync(int id)
        {
            if (id <= 0)
                return false;

            var internUser = await _internUserRepository.GetByIdAsync(id);
            if (internUser == null)
                return false;

            var isInternUserInUse = await IsInternUserInUseAsync(id);
            if (isInternUserInUse)
                throw new InvalidOperationException($"No se puede eliminar el usuario '{internUser.FirstName} {internUser.LastName}' porque está siendo usado por otras entidades del sistema");

            return await _internUserRepository.DeleteAsync(id);
        }

        private async Task<bool> IsInternUserInUseAsync(int internUserId)
        {
            var hasDependencies = await _internUserRepository.HasDependenciesAsync(internUserId);
            return hasDependencies;
        }
    }
}
