using WTB.API.Models.DTOs.Assistance;
using WTB.API.Models.Entities;
using WTB.API.Data.Repositories.Models;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de asistencia que encapsula la lógica de negocio
    /// </summary>
    public interface IAssistanceService
    {
        #region Operaciones CRUD Básicas
        
        /// <summary>
        /// Obtiene una asistencia por ID
        /// </summary>
        Task<Assistance?> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene una asistencia por ID con includes
        /// </summary>
        Task<Assistance?> GetByIdWithIncludesAsync(int id);
        
        /// <summary>
        /// Obtiene todas las asistencias
        /// </summary>
        Task<IEnumerable<Assistance>> GetAllAsync();
        
        /// <summary>
        /// Crea un nuevo registro de asistencia
        /// </summary>
        Task<Assistance> CreateAsync(CheckInDto checkInDto);
        
        /// <summary>
        /// Actualiza un registro de asistencia existente
        /// </summary>
        Task<Assistance> UpdateAsync(int id, UpdateAssistanceDto updateDto);
        
        /// <summary>
        /// Elimina un registro de asistencia
        /// </summary>
        Task DeleteAsync(int id);
        
        #endregion
        
        #region Operaciones de Búsqueda y Filtrado
        
        /// <summary>
        /// Obtiene asistencias con filtros avanzados y paginación
        /// </summary>
        Task<(IEnumerable<Assistance> Items, int TotalCount)> GetFilteredAsync(AssistanceFilterDto filter);
        
        /// <summary>
        /// Obtiene asistencias por empleado en un período
        /// </summary>
        Task<IEnumerable<Assistance>> GetByEmployeeAndPeriodAsync(int employeeId, DateTime fromDate, DateTime toDate);
        
        /// <summary>
        /// Obtiene asistencias por proyecto en un período
        /// </summary>
        Task<IEnumerable<Assistance>> GetByProjectAndPeriodAsync(int projectId, DateTime fromDate, DateTime toDate);
        
        #endregion
        
        #region Operaciones Específicas del Negocio
        
        /// <summary>
        /// Obtiene la asistencia activa de un empleado (check-in sin check-out)
        /// </summary>
        Task<Assistance?> GetActiveAsync(int employeeId);
        
        /// <summary>
        /// Verifica si un empleado tiene asistencia activa
        /// </summary>
        Task<bool> HasActiveAsync(int employeeId);
        
        /// <summary>
        /// Registra el check-in de un empleado
        /// </summary>
        Task<Assistance> CheckInAsync(CheckInDto checkInDto);
        
        /// <summary>
        /// Registra el check-out de un empleado
        /// </summary>
        Task<Assistance> CheckOutAsync(CheckOutDto checkOutDto);
        
        /// <summary>
        /// Obtiene asistencias por rango de horas trabajadas
        /// </summary>
        Task<IEnumerable<Assistance>> GetByWorkHoursRangeAsync(decimal minHours, decimal maxHours, DateTime fromDate, DateTime toDate);
        
        /// <summary>
        /// Obtiene asistencias con tiempo excesivo
        /// </summary>
        Task<IEnumerable<Assistance>> GetExcessiveWorkTimeAsync(DateTime fromDate, DateTime toDate, int maxHours = 12);
        
        #endregion
        
        #region Operaciones de Reportes y Estadísticas
        
        /// <summary>
        /// Obtiene el resumen de horas trabajadas por empleado en un período
        /// </summary>
        Task<IEnumerable<EmployeeWorkSummary>> GetEmployeeWorkSummaryAsync(DateTime fromDate, DateTime toDate);
        
        /// <summary>
        /// Obtiene el resumen de horas trabajadas por proyecto en un período
        /// </summary>
        Task<IEnumerable<ProjectWorkSummary>> GetProjectWorkSummaryAsync(DateTime fromDate, DateTime toDate);
        
        /// <summary>
        /// Obtiene estadísticas de asistencia en un período
        /// </summary>
        Task<AssistanceStatistics> GetStatisticsAsync(DateTime fromDate, DateTime toDate);
        
        /// <summary>
        /// Obtiene empleados con mayor tiempo trabajado en un período
        /// </summary>
        Task<IEnumerable<TopWorkerSummary>> GetTopWorkersAsync(DateTime fromDate, DateTime toDate, int top = 10);
        
        #endregion
        
        #region Operaciones de Validación y Corrección
        
        /// <summary>
        /// Obtiene asistencias que requieren corrección administrativa
        /// </summary>
        Task<IEnumerable<Assistance>> GetNeedingCorrectionAsync();
        
        /// <summary>
        /// Valida si una asistencia puede ser creada
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(CheckInDto checkInDto);
        
        /// <summary>
        /// Valida si una asistencia puede ser actualizada
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(int id, UpdateAssistanceDto updateDto);
        
        /// <summary>
        /// Valida si una asistencia puede ser eliminada
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForDeletionAsync(int id);
        
        /// <summary>
        /// Valida si un check-in puede ser registrado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateCheckInAsync(CheckInDto checkInDto);
        
        /// <summary>
        /// Valida si un check-out puede ser registrado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateCheckOutAsync(CheckOutDto checkOutDto);
        
        #endregion
    }
}
