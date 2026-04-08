using WorkTrackBio.API.DataTransferObjects.Employee;

namespace WorkTrackBio.API.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeListDto>> GetAllAsync();
        Task<EmployeeDetailDto> GetByIdAsync(int id);
        Task<EmployeeDetailDto> CreateAsync(EmployeeCreateDto dto);
        Task<EmployeeDetailDto> UpdateAsync(int id, EmployeeUpdateDto dto);
        Task DeleteAsync(int id);
    }
}