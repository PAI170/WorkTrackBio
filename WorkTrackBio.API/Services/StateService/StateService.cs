using AutoMapper;
using WorkTrackBio.API.DataTransferObjects.State;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Services.StateService
{
    public class StateService : IStateService
    {
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public StateService(IStateRepository stateRepository, IMapper mapper)
        {
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<StateDataTransferObject>> GetAllStatesAsync()
        {
            var states = await _stateRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<StateDataTransferObject>>(states);
        }

        public async Task<StateDataTransferObject?> GetStateByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var state = await _stateRepository.GetByIdAsync(id);
            return _mapper.Map<StateDataTransferObject>(state);
        }

        public async Task<IEnumerable<StateDataTransferObject>> GetStatesByTypeAsync(string stateType)
        {
            if (string.IsNullOrWhiteSpace(stateType))
                throw new ArgumentException("El tipo de estado no puede estar vacío", nameof(stateType));

            var states = await _stateRepository.GetByTypeAsync(stateType);
            return _mapper.Map<IEnumerable<StateDataTransferObject>>(states);
        }

        public async Task<bool> StateExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            var state = await _stateRepository.GetByIdAsync(id);
            return state != null;
        }

        public async Task<bool> StateNameExistsAsync(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName))
                return false;

            return await _stateRepository.ExistsByNameAsync(stateName);
        }

        public async Task<StateDataTransferObject> CreateStateAsync(CreateStateDataTransferObject createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            // Verificar si ya existe un estado con el mismo nombre
            var existingState = await _stateRepository.GetAllAsync();
            if (existingState.Any(s => string.Equals(s.StateName, createDto.StateName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ya existe un estado con el nombre '{createDto.StateName}' (ignorando mayúsculas/minúsculas)");

            // Mapear DTO a Model
            var state = _mapper.Map<WorkTrackBio.API.Data.Models.State>(createDto);

            // Crear el estado
            var createdState = await _stateRepository.CreateAsync(state);

            // Mapear de vuelta a DTO y retornar
            return _mapper.Map<StateDataTransferObject>(createdState);
        }

        public async Task<StateDataTransferObject?> UpdateStateAsync(UpdateStateDataTransferObject updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(updateDto.Id));

            // Verificar si el estado existe
            var existingState = await _stateRepository.GetByIdAsync(updateDto.Id);
            if (existingState == null)
                return null;

            bool hasChanges = false;

            // Actualizar StateName solo si se proporcionó un nuevo valor
            if (!string.IsNullOrWhiteSpace(updateDto.StateName))
            {
                // Verificar si el nuevo nombre ya existe en otro estado
                var allStates = await _stateRepository.GetAllAsync();
                if (allStates.Any(s => string.Equals(s.StateName, updateDto.StateName, StringComparison.OrdinalIgnoreCase) && 
                    s.Id != updateDto.Id))
                    throw new InvalidOperationException($"Ya existe un estado con el nombre '{updateDto.StateName}' (ignorando mayúsculas/minúsculas)");

                // Solo actualizar si realmente cambió
                if (!string.Equals(existingState.StateName, updateDto.StateName, StringComparison.OrdinalIgnoreCase))
                {
                    existingState.StateName = updateDto.StateName;
                    hasChanges = true;
                }
            }

            // Actualizar StateType solo si se proporcionó un nuevo valor
            if (!string.IsNullOrWhiteSpace(updateDto.StateType))
            {
                if (!string.Equals(existingState.StateType, updateDto.StateType, StringComparison.OrdinalIgnoreCase))
                {
                    existingState.StateType = updateDto.StateType;
                    hasChanges = true;
                }
            }

            // Actualizar Description solo si se proporcionó un nuevo valor
            if (updateDto.Description != null)
            {
                if (!string.Equals(existingState.Description ?? "", updateDto.Description))
                {
                    existingState.Description = updateDto.Description;
                    hasChanges = true;
                }
            }

            // Si no hay cambios, retornar el estado existente sin modificar
            if (!hasChanges)
            {
                return _mapper.Map<StateDataTransferObject>(existingState);
            }

            // Guardar cambios solo si hubo modificaciones
            var updatedState = await _stateRepository.UpdateAsync(existingState);

            // Mapear de vuelta a DTO y retornar
            return _mapper.Map<StateDataTransferObject>(updatedState);
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            if (id <= 0)
                return false;

            // Verificar si el estado existe
            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return false;

            // Verificar si el estado está siendo usado por otras entidades
            // Esto evita errores de Foreign Key constraint
            var isStateInUse = await IsStateInUseAsync(id);
            if (isStateInUse)
                throw new InvalidOperationException($"No se puede eliminar el estado '{state.StateName}' porque está siendo usado por otras entidades del sistema");

            return await _stateRepository.DeleteAsync(id);
        }

        private async Task<bool> IsStateInUseAsync(int stateId)
        {
            // Verificar si hay entidades que dependen de este estado
            var hasDependencies = await _stateRepository.HasDependenciesAsync(stateId);
            return hasDependencies;
        }
    }
}
