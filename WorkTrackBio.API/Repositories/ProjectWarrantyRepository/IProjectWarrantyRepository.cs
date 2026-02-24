using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectWarrantyRepository
{
    public interface IProjectWarrantyRepository
    {
        Task<IEnumerable<ProjectWarranty>> GetAllAsync();
        Task<ProjectWarranty?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectWarranty>> GetByProjectAsync(int projectId);
        Task<IEnumerable<ProjectWarranty>> GetByEmployeeAsync(int employeeId);
        Task<IEnumerable<ProjectWarranty>> GetByStateAsync(int stateId);
        Task<IEnumerable<ProjectWarranty>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ProjectWarranty>> GetByProjectAndStateAsync(int projectId, int stateId);
        Task<ProjectWarranty> CreateAsync(ProjectWarranty projectWarranty);
        Task<ProjectWarranty> UpdateAsync(ProjectWarranty projectWarranty);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int projectWarrantyId);
        Task<int> SaveChangesAsync();
    }
}
