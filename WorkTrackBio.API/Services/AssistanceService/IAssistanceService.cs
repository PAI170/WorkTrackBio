using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Assistance;

namespace WorkTrackBio.API.Services.AssistanceService
{
    public interface IAssistanceService
    {
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAllAssistancesAsync();
        Task<ApiResponse<AssistanceDataTransferObject>> GetAssistanceByIdAsync(int id);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAsync(int employeeId);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAsync(int projectId);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByDateAsync(DateOnly date);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAndProjectAsync(int employeeId, int projectId);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAndDateRangeAsync(int projectId, DateOnly startDate, DateOnly endDate);
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAndEmployeeAsync(int projectId, int employeeId);
        Task<ApiResponse<AssistanceDataTransferObject>> CreateAssistanceAsync(CreateAssistanceDataTransferObject createDto);
        Task<ApiResponse<AssistanceDataTransferObject>> UpdateAssistanceAsync(int id, UpdateAssistanceDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteAssistanceAsync(int id);
        Task<ApiResponse<decimal>> CalculateTotalHoursAsync(int assistanceId);
    }
}
