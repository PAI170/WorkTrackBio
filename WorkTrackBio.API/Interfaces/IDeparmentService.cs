using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentResponseDto>> GetAllAsync();
        Task<DepartmentResponseDto> GetByIdAsync(int id);
        Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto);
        Task<DepartmentResponseDto> UpdateAsync(int id, DepartmentUpdateDto dto);
        Task DeleteAsync(int id);
    }
}