using WorkTrackBio.API.DataTransferObjects.Role;

namespace WorkTrackBio.API.Services.RoleService
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDataTransferObject>> GetAllRolesAsync();
        Task<RoleDataTransferObject?> GetRoleByIdAsync(int id);
        Task<RoleDataTransferObject?> GetRoleByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(int id);
        Task<bool> RoleNameExistsAsync(string roleName);
        Task<RoleDataTransferObject> CreateRoleAsync(CreateRoleDataTransferObject createDto);
        Task<RoleDataTransferObject?> UpdateRoleAsync(UpdateRoleDataTransferObject updateDto);
        Task<bool> DeleteRoleAsync(int id);
    }
}
