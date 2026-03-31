using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectMaintenanceRepository
{
    public interface IProjectMaintenanceRepository
    {
        Task<IEnumerable<ProjectMaintenance>> GetAllAsync();
        Task<ProjectMaintenance?> GetByIdAsync(int id);
        Task<IEnumerable<ProjectMaintenance>> GetByProjectAsync(int projectId);
        Task<IEnumerable<ProjectMaintenance>> GetByEmployeeAsync(int employeeId);
        Task<IEnumerable<ProjectMaintenance>> GetByStateAsync(int stateId);
        Task<ProjectMaintenance> CreateAsync(ProjectMaintenance projectMaintenance);
        Task<ProjectMaintenance?> UpdateAsync(ProjectMaintenance projectMaintenance);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> HasDependenciesAsync(int id);
    }
}
