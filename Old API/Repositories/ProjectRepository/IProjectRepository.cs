using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectRepository
{
    public interface IProjectRepository
    {
        Task<IEnumerable<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<Project?> GetByNameAsync(string projectName);
        Task<bool> ExistsByNameAsync(string projectName);
        Task<IEnumerable<Project>> GetByStateAsync(int stateId);
        Task<Project> CreateAsync(Project project);
        Task<Project> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int projectId);
        Task<int> SaveChangesAsync();
    }
}
