using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Services.ProjectWarrantyService
{
    public interface IProjectWarrantyService
    {
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetAllProjectWarrantiesAsync();
        Task<ApiResponse<ProjectWarrantyDataTransferObject>> GetProjectWarrantyByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAsync(int projectId);
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByEmployeeAsync(int employeeId);
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByStateAsync(int stateId);
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAndStateAsync(int projectId, int stateId);
        Task<ApiResponse<ProjectWarrantyDataTransferObject>> CreateProjectWarrantyAsync(CreateProjectWarrantyDataTransferObject createDto);
        Task<ApiResponse<ProjectWarrantyDataTransferObject>> UpdateProjectWarrantyAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteProjectWarrantyAsync(int id);
    }
}
