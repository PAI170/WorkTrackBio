using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Data.Repositories.Models;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de empleados con métodos específicos del negocio
    /// </summary>
    public interface IEmployeeRepository : IRepository<EmployeeInfo>
    {
        /// <summary>
        /// Obtiene un empleado por número de documento
        /// </summary>
        Task<EmployeeInfo?> GetByDocumentNumberAsync(string documentNumber);

        /// <summary>
        /// Obtiene un empleado por número de documento con includes
        /// </summary>
        Task<EmployeeInfo?> GetByDocumentNumberWithIncludesAsync(string documentNumber);

        /// <summary>
        /// Verifica si existe un empleado con el número de documento
        /// </summary>
        Task<bool> ExistsByDocumentNumberAsync(string documentNumber);

        /// <summary>
        /// Obtiene empleados por tipo de documento
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetByDocumentTypeAsync(int documentTypeId);

        /// <summary>
        /// Obtiene empleados por estado
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene empleados activos
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetActiveEmployeesAsync();

        /// <summary>
        /// Obtiene empleados con huella dactilar
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetEmployeesWithFingerPrintAsync();

        /// <summary>
        /// Obtiene empleados por rango de edad
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetByAgeRangeAsync(int minAge, int maxAge);

        /// <summary>
        /// Obtiene empleados por rango de costo por hora
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetByCostPerHourRangeAsync(decimal minCost, decimal maxCost);

        /// <summary>
        /// Busca empleados por nombre o apellido
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> SearchByNameAsync(string searchTerm);

        /// <summary>
        /// Obtiene empleados con paginación y filtros avanzados
        /// </summary>
        Task<(IEnumerable<EmployeeInfo> Items, int TotalCount)> GetFilteredAsync(
            EmployeeFilterDto filter,
            int pageNumber = 1,
            int pageSize = 10);

        /// <summary>
        /// Obtiene estadísticas de empleados
        /// </summary>
        Task<EmployeeStatistics> GetEmployeeStatisticsAsync();

        /// <summary>
        /// Obtiene empleados que no han registrado asistencia en un período
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetEmployeesWithoutAssistanceAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene empleados con mayor tiempo trabajado en un período
        /// </summary>
        Task<IEnumerable<EmployeeWorkSummary>> GetTopWorkingEmployeesAsync(DateTime fromDate, DateTime toDate, int top = 10);
    }
}
