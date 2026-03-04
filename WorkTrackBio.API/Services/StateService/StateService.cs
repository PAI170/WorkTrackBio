using AutoMapper;
using WorkTrackBio.API.Common;
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

        public async Task<ApiResponse<IEnumerable<StateDataTransferObject>>> GetAllStatesAsync()
        {
            var states = await _stateRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<StateDataTransferObject>>(states);
            return ApiResponse<IEnumerable<StateDataTransferObject>>.SuccessResponse(result, "Estados obtenidos exitosamente");
        }

        public async Task<ApiResponse<StateDataTransferObject>> GetStateByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<StateDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var state = await _stateRepository.GetByIdAsync(id);

            if (state == null)
                return ApiResponse<StateDataTransferObject>.ErrorResponse($"No se encontró un estado con ID {id}", 404);

            var result = _mapper.Map<StateDataTransferObject>(state);
            return ApiResponse<StateDataTransferObject>.SuccessResponse(result, "Estado obtenido exitosamente");
        }

        public async Task<ApiResponse<IEnumerable<StateDataTransferObject>>> GetStatesByTypeAsync(string stateType)
        {
            if (string.IsNullOrWhiteSpace(stateType))
                return ApiResponse<IEnumerable<StateDataTransferObject>>.ErrorResponse("El tipo de estado no puede estar vacío", 400);

            var states = await _stateRepository.GetByTypeAsync(stateType);
            var result = _mapper.Map<IEnumerable<StateDataTransferObject>>(states);
            return ApiResponse<IEnumerable<StateDataTransferObject>>.SuccessResponse(result, "Estados obtenidos por tipo exitosamente");
        }

        public async Task<ApiResponse<bool>> StateExistsAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var state = await _stateRepository.GetByIdAsync(id);
            var exists = state != null;
            return ApiResponse<bool>.SuccessResponse(exists, exists ? "El estado existe" : "El estado no existe");
        }

        public async Task<ApiResponse<bool>> StateNameExistsAsync(string stateName)
        {
            if (string.IsNullOrWhiteSpace(stateName))
                return ApiResponse<bool>.ErrorResponse("El nombre del estado no puede estar vacío", 400);

            var exists = await _stateRepository.ExistsByNameAsync(stateName);
            return ApiResponse<bool>.SuccessResponse(exists, exists ? $"Ya existe un estado con el nombre '{stateName}'" : $"No existe un estado con el nombre '{stateName}'");
        }

        public async Task<ApiResponse<StateDataTransferObject>> CreateStateAsync(CreateStateDataTransferObject createDto)
        {
            if (createDto == null)
                return ApiResponse<StateDataTransferObject>.ErrorResponse("Los datos del estado no pueden estar vacíos", 400);

            if (await _stateRepository.ExistsByNameAsync(createDto.StateName))
                return ApiResponse<StateDataTransferObject>.ErrorResponse($"Ya existe un estado con el nombre '{createDto.StateName}'", 409);

            var state = _mapper.Map<WorkTrackBio.API.Data.Models.State>(createDto);
            var createdState = await _stateRepository.CreateAsync(state);
            var result = _mapper.Map<StateDataTransferObject>(createdState);

            return ApiResponse<StateDataTransferObject>.SuccessResponse(result, "Estado creado exitosamente", 201);
        }

        public async Task<ApiResponse<StateDataTransferObject>> UpdateStateAsync(UpdateStateDataTransferObject updateDto)
        {
            if (updateDto == null)
                return ApiResponse<StateDataTransferObject>.ErrorResponse("Los datos del estado no pueden estar vacíos", 400);

            if (updateDto.Id <= 0)
                return ApiResponse<StateDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var existingState = await _stateRepository.GetByIdAsync(updateDto.Id);
            if (existingState == null)
                return ApiResponse<StateDataTransferObject>.ErrorResponse($"No se encontró un estado con ID {updateDto.Id}", 404);

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(updateDto.StateName))
            {
                if (await _stateRepository.ExistsByNameAsync(updateDto.StateName) && existingState.StateName.ToLower() != updateDto.StateName.ToLower())
                    return ApiResponse<StateDataTransferObject>.ErrorResponse($"Ya existe un estado con el nombre '{updateDto.StateName}'", 409);

                if (!string.Equals(existingState.StateName, updateDto.StateName, StringComparison.OrdinalIgnoreCase))
                {
                    existingState.StateName = updateDto.StateName;
                    hasChanges = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(updateDto.StateType))
            {
                if (!string.Equals(existingState.StateType, updateDto.StateType, StringComparison.OrdinalIgnoreCase))
                {
                    existingState.StateType = updateDto.StateType;
                    hasChanges = true;
                }
            }

            if (updateDto.Description != null)
            {
                if (!string.Equals(existingState.Description ?? "", updateDto.Description))
                {
                    existingState.Description = updateDto.Description;
                    hasChanges = true;
                }
            }

            if (!hasChanges)
            {
                var unchanged = _mapper.Map<StateDataTransferObject>(existingState);
                return ApiResponse<StateDataTransferObject>.SuccessResponse(unchanged, "No se detectaron cambios");
            }

            var updatedState = await _stateRepository.UpdateAsync(existingState);
            var result = _mapper.Map<StateDataTransferObject>(updatedState);
            return ApiResponse<StateDataTransferObject>.SuccessResponse(result, "Estado actualizado exitosamente");
        }

        public async Task<ApiResponse<bool>> DeleteStateAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var state = await _stateRepository.GetByIdAsync(id);
            if (state == null)
                return ApiResponse<bool>.ErrorResponse($"No se encontró un estado con ID {id}", 404);

            var isInUse = await _stateRepository.HasDependenciesAsync(id);
            if (isInUse)
                return ApiResponse<bool>.ErrorResponse($"No se puede eliminar el estado '{state.StateName}' porque está siendo usado por otras entidades del sistema", 409);

            await _stateRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Estado eliminado exitosamente");
        }
    }
}