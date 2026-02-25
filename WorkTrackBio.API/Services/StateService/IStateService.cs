using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Services.StateService
{
    public interface IStateService
    {
        Task<ApiResponse<IEnumerable<StateDataTransferObject>>> GetAllStatesAsync();
        Task<ApiResponse<StateDataTransferObject>> GetStateByIdAsync(int id);
        Task<ApiResponse<IEnumerable<StateDataTransferObject>>> GetStatesByTypeAsync(string stateType);
        Task<ApiResponse<bool>> StateExistsAsync(int id);
        Task<ApiResponse<bool>> StateNameExistsAsync(string stateName);
        Task<ApiResponse<StateDataTransferObject>> CreateStateAsync(CreateStateDataTransferObject createDto);
        Task<ApiResponse<StateDataTransferObject>> UpdateStateAsync(UpdateStateDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteStateAsync(int id);
    }
}