using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.AssistanceRepository
{
    public interface IAssistanceRepository
    {
        Task<IEnumerable<Assistance>> GetAllAsync();
        Task<Assistance?> GetByIdAsync(int id);
        Task<IEnumerable<Assistance>> GetByEmployeeAsync(int employeeId);
        Task<IEnumerable<Assistance>> GetByProjectAsync(int projectId);
        Task<IEnumerable<Assistance>> GetByDateAsync(DateOnly date);
        Task<IEnumerable<Assistance>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Assistance>> GetByEmployeeAndProjectAsync(int employeeId, int projectId);
        Task<IEnumerable<Assistance>> GetByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Assistance>> GetByProjectAndDateRangeAsync(int projectId, DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<Assistance>> GetByProjectAndEmployeeAsync(int projectId, int employeeId);
        Task<bool> HasOpenCheckInAsync(int employeeId);
        Task<Assistance> CreateAsync(Assistance assistance);
        Task<Assistance> UpdateAsync(Assistance assistance);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int assistanceId);
        Task<int> SaveChangesAsync();
    }
}
