using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;

namespace WorkTrackBio.API.Services.ProjectMaintenanceService
{
    public interface IProjectMaintenanceService
    {
        Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetAllProjectMaintenancesAsync();
        Task<ApiResponse<ProjectMaintenanceDataTransferObject>> GetProjectMaintenanceByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenancesByProjectAsync(int projectId);
        Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenancesByEmployeeAsync(int employeeId);
        Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenancesByStateAsync(int stateId);
        Task<ApiResponse<ProjectMaintenanceDataTransferObject>> CreateProjectMaintenanceAsync(CreateProjectMaintenanceDataTransferObject createDto);
        Task<ApiResponse<ProjectMaintenanceDataTransferObject>> UpdateProjectMaintenanceAsync(UpdateProjectMaintenanceDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteProjectMaintenanceAsync(int id);
    }
}
