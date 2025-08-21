using WorkTrackBio.API.DataTransferObjects.Project;

namespace WorkTrackBio.API.Services.ProjectService
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDataTransferObject>> GetAllProjectsAsync();
        Task<ProjectDataTransferObject?> GetProjectByIdAsync(int id);
        Task<ProjectDataTransferObject?> GetProjectByNameAsync(string projectName);
        Task<IEnumerable<ProjectDataTransferObject>> GetProjectsByStateAsync(int stateId);
        Task<bool> ProjectExistsAsync(int id);
        Task<bool> ProjectNameExistsAsync(string projectName);
        Task<ProjectDataTransferObject> CreateProjectAsync(CreateProjectDataTransferObject createDto);
        Task<ProjectDataTransferObject?> UpdateProjectAsync(UpdateProjectDataTransferObject updateDto);
        Task<bool> DeleteProjectAsync(int id);
    }
}
