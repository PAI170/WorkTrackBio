using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.EmployeeInfoRepository
{
    public interface IEmployeeInfoRepository
    {
        Task<IEnumerable<EmployeeInfo>> GetAllAsync();
        Task<EmployeeInfo?> GetByIdAsync(int id);
        Task<EmployeeInfo?> GetByDocumentNumberAsync(string documentNumber, int documentTypeId);
        Task<bool> ExistsByDocumentNumberAsync(string documentNumber, int documentTypeId);
        Task<IEnumerable<EmployeeInfo>> GetByStateAsync(int stateId);
        Task<IEnumerable<EmployeeInfo>> GetByDocumentTypeAsync(int documentTypeId);
        Task<EmployeeInfo> CreateAsync(EmployeeInfo employeeInfo);
        Task<EmployeeInfo> UpdateAsync(EmployeeInfo employeeInfo);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int employeeInfoId);
        Task<int> SaveChangesAsync();
    }
}
