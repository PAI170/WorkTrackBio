using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Project;

namespace WorkTrackBio.API.Services.ProjectService
{
    public interface IProjectService
    {
        Task<ApiResponse<IEnumerable<ProjectDataTransferObject>>> GetAllProjectsAsync();
        Task<ApiResponse<ProjectDataTransferObject>> GetProjectByIdAsync(int id);
        Task<ApiResponse<ProjectDataTransferObject>> GetProjectByNameAsync(string projectName);
        Task<ApiResponse<IEnumerable<ProjectDataTransferObject>>> GetProjectsByStateAsync(int stateId);
        Task<ApiResponse<bool>> ProjectExistsAsync(int id);
        Task<ApiResponse<bool>> ProjectNameExistsAsync(string projectName);
        Task<ApiResponse<ApiResponse<ProjectDataTransferObject>>> CreateProjectAsync(CreateProjectDataTransferObject createDto);
        Task<ApiResponse<ProjectDataTransferObject>> UpdateProjectAsync(UpdateProjectDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteProjectAsync(int id);
    }
}
