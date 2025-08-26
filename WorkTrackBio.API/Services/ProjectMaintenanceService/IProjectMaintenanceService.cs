using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;

namespace WorkTrackBio.API.Services.ProjectMaintenanceService
{
    public interface IProjectMaintenanceService
    {
        Task<IEnumerable<ProjectMaintenanceDataTransferObject>> GetAllProjectMaintenancesAsync();
        Task<ProjectMaintenanceDataTransferObject?> GetProjectMaintenanceByIdAsync(int id);
        Task<IEnumerable<ProjectMaintenanceDataTransferObject>> GetProjectMaintenancesByProjectAsync(int projectId);
        Task<IEnumerable<ProjectMaintenanceDataTransferObject>> GetProjectMaintenancesByEmployeeAsync(int employeeId);
        Task<IEnumerable<ProjectMaintenanceDataTransferObject>> GetProjectMaintenancesByStateAsync(int stateId);
        Task<ProjectMaintenanceDataTransferObject> CreateProjectMaintenanceAsync(CreateProjectMaintenanceDataTransferObject createDto);
        Task<ProjectMaintenanceDataTransferObject?> UpdateProjectMaintenanceAsync(UpdateProjectMaintenanceDataTransferObject updateDto);
        Task<bool> DeleteProjectMaintenanceAsync(int id);
        Task<bool> ProjectMaintenanceExistsAsync(int id);
    }
}
