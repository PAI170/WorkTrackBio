using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.State;

namespace WorkTrackBio.API.Repositories.StateRepository
{
    // Interface Repository, What can be do in the DB
    public interface IStateRepository
    {
        Task<IEnumerable<State>> GetAllAsync();
        Task<State?> GetByIdAsync(int id);
        Task<IEnumerable<State>> GetByTypeAsync(string stateType);
        Task<bool> ExistsByNameAsync(string stateName);
        Task<State> CreateAsync(State state);
        Task<State> UpdateAsync(State state);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int stateId);
        Task<int> SaveChangesAsync();
    }
}
