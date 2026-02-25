using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;

namespace WorkTrackBio.API.Services.EmployeeInfoService
{
    public interface IEmployeeInfoService
    {
        Task<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>> GetAllEmployeesAsync();
        Task<ApiResponse<EmployeeInfoDataTransferObject>> GetEmployeeByIdAsync(int id);
        Task<ApiResponse<EmployeeInfoDataTransferObject>> GetEmployeeByDocumentNumberAsync(string documentNumber, int documentTypeId);
        Task<ApiResponse<bool>> EmployeeExistsAsync(int id);
        Task<ApiResponse<bool>> EmployeeDocumentExistsAsync(string documentNumber, int documentTypeId);
        Task<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>> GetEmployeesByStateAsync(int stateId);
        Task<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>> GetEmployeesByDocumentTypeAsync(int documentTypeId);
        Task<ApiResponse<EmployeeInfoDataTransferObject>> CreateEmployeeAsync(CreateEmployeeInfoDataTransferObject createDto);
        Task<ApiResponse<EmployeeInfoDataTransferObject>> UpdateEmployeeAsync(UpdateEmployeeInfoDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteEmployeeAsync(int id);
    }
}