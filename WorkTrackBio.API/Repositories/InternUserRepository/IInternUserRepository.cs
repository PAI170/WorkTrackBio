using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.InternUserRepository
{
    // Interface Repository, What can be do in the DB
    public interface IInternUserRepository
    {
        Task<IEnumerable<InternUser>> GetAllAsync();
        Task<InternUser?> GetByIdAsync(int id);
        Task<InternUser?> GetByEmailAsync(string email);
        Task<IEnumerable<InternUser>> GetByRoleAsync(int roleId);
        Task<IEnumerable<InternUser>> GetByStateAsync(int stateId);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByIdAsync(int id);
        Task<InternUser> CreateAsync(InternUser internUser);
        Task<InternUser> UpdateAsync(InternUser internUser);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int internUserId);
        Task<int> SaveChangesAsync();
    }
}
