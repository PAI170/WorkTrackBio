using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDto>> GetAllAsync();
        Task<RoleResponseDto> GetByIdAsync(int id);
        Task<RoleResponseDto> CreateAsync(RoleCreateDto dto);
        Task<RoleResponseDto> UpdateAsync(int id, RoleUpdateDto dto);
        Task DeleteAsync(int id);
    }
}