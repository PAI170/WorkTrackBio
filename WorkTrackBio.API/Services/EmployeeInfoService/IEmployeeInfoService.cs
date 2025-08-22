using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;

namespace WorkTrackBio.API.Services.EmployeeInfoService
{
    public interface IEmployeeInfoService
    {
        Task<IEnumerable<EmployeeInfoDataTransferObject>> GetAllEmployeesAsync();
        Task<EmployeeInfoDataTransferObject?> GetEmployeeByIdAsync(int id);
        Task<EmployeeInfoDataTransferObject?> GetEmployeeByDocumentNumberAsync(string documentNumber, int documentTypeId);
        Task<bool> EmployeeExistsAsync(int id);
        Task<bool> EmployeeDocumentExistsAsync(string documentNumber, int documentTypeId);
        Task<IEnumerable<EmployeeInfoDataTransferObject>> GetEmployeesByStateAsync(int stateId);
        Task<IEnumerable<EmployeeInfoDataTransferObject>> GetEmployeesByDocumentTypeAsync(int documentTypeId);
        Task<EmployeeInfoDataTransferObject> CreateEmployeeAsync(CreateEmployeeInfoDataTransferObject createDto);
        Task<EmployeeInfoDataTransferObject?> UpdateEmployeeAsync(UpdateEmployeeInfoDataTransferObject updateDto);
        Task<bool> DeleteEmployeeAsync(int id);
    }
}
