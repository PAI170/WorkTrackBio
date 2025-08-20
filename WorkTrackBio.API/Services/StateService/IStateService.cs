using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Services.StateService
{
    public interface IStateService
    {
        Task<IEnumerable<StateDataTransferObject>> GetAllStatesAsync();
        Task<StateDataTransferObject?> GetStateByIdAsync(int id);
        Task<IEnumerable<StateDataTransferObject>> GetStatesByTypeAsync(string stateType);
        Task<bool> StateExistsAsync(int id);
        Task<bool> StateNameExistsAsync(string stateName);
        Task<StateDataTransferObject> CreateStateAsync(CreateStateDataTransferObject createDto);
        Task<StateDataTransferObject?> UpdateStateAsync(UpdateStateDataTransferObject updateDto);
        Task<bool> DeleteStateAsync(int id);
    }
}
