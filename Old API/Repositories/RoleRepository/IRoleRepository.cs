using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.RoleRepository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> ExistsByNameAsync(string roleName);
        Task<Role> CreateAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
        Task<int> SaveChangesAsync();
    }
}
