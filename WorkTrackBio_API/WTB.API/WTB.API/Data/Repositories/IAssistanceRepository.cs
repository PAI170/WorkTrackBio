using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Data.Repositories.Models;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de asistencia con métodos específicos del negocio
    /// </summary>
    public interface IAssistanceRepository : IRepository<Assistance>
    {
        /// <summary>
        /// Obtiene la asistencia activa de un empleado (check-in sin check-out)
        /// </summary>
        Task<Assistance?> GetActiveAssistanceAsync(int employeeId);

        /// <summary>
        /// Verifica si un empleado tiene asistencia activa
        /// </summary>
        Task<bool> HasActiveAssistanceAsync(int employeeId);

        /// <summary>
        /// Obtiene todas las asistencias de un empleado en un período
        /// </summary>
        Task<IEnumerable<Assistance>> GetByEmployeeAndPeriodAsync(int employeeId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene todas las asistencias de un proyecto en un período
        /// </summary>
        Task<IEnumerable<Assistance>> GetByProjectAndPeriodAsync(int projectId, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene asistencias con filtros avanzados y paginación
        /// </summary>
        Task<(IEnumerable<Assistance> Items, int TotalCount)> GetFilteredAsync(
            AssistanceFilterDto filter,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Obtiene el resumen de horas trabajadas por empleado en un período
        /// </summary>
        Task<IEnumerable<EmployeeWorkSummary>> GetEmployeeWorkSummaryAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene el resumen de horas trabajadas por proyecto en un período
        /// </summary>
        Task<IEnumerable<ProjectWorkSummary>> GetProjectWorkSummaryAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene asistencias que requieren corrección administrativa
        /// </summary>
        Task<IEnumerable<Assistance>> GetAssistancesNeedingCorrectionAsync();

        /// <summary>
        /// Obtiene estadísticas de asistencia en un período
        /// </summary>
        Task<AssistanceStatistics> GetAssistanceStatisticsAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene empleados con mayor tiempo trabajado en un período
        /// </summary>
        Task<IEnumerable<TopWorkerSummary>> GetTopWorkersAsync(DateTime fromDate, DateTime toDate, int top = 10);

        /// <summary>
        /// Obtiene asistencias por rango de horas
        /// </summary>
        Task<IEnumerable<Assistance>> GetByWorkHoursRangeAsync(decimal minHours, decimal maxHours, DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene asistencias con tiempo excesivo (más de 12 horas)
        /// </summary>
        Task<IEnumerable<Assistance>> GetExcessiveWorkTimeAsync(DateTime fromDate, DateTime toDate, int maxHours = 12);
    }
}
