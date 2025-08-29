using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Assistance;

namespace WorkTrackBio.API.Services.AssistanceService
{
    /// <summary>
    /// Interfaz para el servicio de Assistance
    /// </summary>
    public interface IAssistanceService
    {
        /// <summary>
        /// Obtiene todos los registros de asistencia
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAllAssistancesAsync();

        /// <summary>
        /// Obtiene un registro de asistencia por ID
        /// </summary>
        Task<ApiResponse<AssistanceDataTransferObject>> GetAssistanceByIdAsync(int id);

        /// <summary>
        /// Obtiene registros de asistencia por empleado
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene registros de asistencia por proyecto
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene registros de asistencia por fecha
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByDateAsync(DateOnly date);

        /// <summary>
        /// Obtiene registros de asistencia por rango de fechas
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByDateRangeAsync(DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Obtiene registros de asistencia por empleado y proyecto
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAndProjectAsync(int employeeId, int projectId);

        /// <summary>
        /// Obtiene registros de asistencia por empleado y rango de fechas
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Obtiene registros de asistencia por proyecto y rango de fechas
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAndDateRangeAsync(int projectId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Obtiene registros de asistencia por proyecto y empleado específico
        /// </summary>
        Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAndEmployeeAsync(int projectId, int employeeId);

        /// <summary>
        /// Crea un nuevo registro de asistencia
        /// </summary>
        Task<ApiResponse<AssistanceDataTransferObject>> CreateAssistanceAsync(CreateAssistanceDataTransferObject createDto);

        /// <summary>
        /// Actualiza un registro de asistencia existente
        /// </summary>
        Task<ApiResponse<AssistanceDataTransferObject>> UpdateAssistanceAsync(int id, UpdateAssistanceDataTransferObject updateDto);

        /// <summary>
        /// Elimina un registro de asistencia
        /// </summary>
        Task<ApiResponse<bool>> DeleteAssistanceAsync(int id);

        /// <summary>
        /// Calcula las horas totales de un registro de asistencia
        /// </summary>
        Task<ApiResponse<decimal>> CalculateTotalHoursAsync(int assistanceId);
    }
}
