using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Role;

namespace WorkTrackBio.API.Services.RoleService
{
    public interface IRoleService
    {
        Task<ApiResponse<IEnumerable<RoleDataTransferObject>>> GetAllRolesAsync();
        Task<ApiResponse<RoleDataTransferObject>> GetRoleByIdAsync(int id);
        Task<ApiResponse<RoleDataTransferObject>> GetRoleByNameAsync(string roleName);
        Task<ApiResponse<bool>> RoleExistsAsync(int id);
        Task<ApiResponse<bool>> RoleNameExistsAsync(string roleName);
        Task<ApiResponse<RoleDataTransferObject>> CreateRoleAsync(CreateRoleDataTransferObject createDto);
        Task<ApiResponse<RoleDataTransferObject>> UpdateRoleAsync(UpdateRoleDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteRoleAsync(int id);
    }
}