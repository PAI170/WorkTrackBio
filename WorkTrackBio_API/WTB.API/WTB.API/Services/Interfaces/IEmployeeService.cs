using WTB.API.Models.DTOs.Employee;
using WTB.API.Models.Entities;
using WTB.API.Data.Repositories.Models;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de empleados que encapsula la lógica de negocio
    /// </summary>
    public interface IEmployeeService
    {
        #region Operaciones CRUD Básicas
        
        /// <summary>
        /// Obtiene un empleado por ID
        /// </summary>
        Task<EmployeeInfo?> GetByIdAsync(int id);
        
        /// <summary>
        /// Obtiene un empleado por ID con includes
        /// </summary>
        Task<EmployeeInfo?> GetByIdWithIncludesAsync(int id);
        
        /// <summary>
        /// Obtiene todos los empleados
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetAllAsync();
        
        /// <summary>
        /// Crea un nuevo empleado
        /// </summary>
        Task<EmployeeInfo> CreateAsync(CreateEmployeeDto createDto);
        
        /// <summary>
        /// Actualiza un empleado existente
        /// </summary>
        Task<EmployeeInfo> UpdateAsync(int id, UpdateEmployeeDto updateDto);
        
        /// <summary>
        /// Elimina un empleado
        /// </summary>
        Task DeleteAsync(int id);
        
        #endregion
        
        #region Operaciones de Búsqueda y Filtrado
        
        /// <summary>
        /// Obtiene empleados con filtros avanzados y paginación
        /// </summary>
        Task<(IEnumerable<EmployeeInfo> Items, int TotalCount)> GetFilteredAsync(EmployeeFilterDto filter);
        
        /// <summary>
        /// Busca empleados por término de búsqueda
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> SearchAsync(string searchTerm);
        
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
        Task<IEnumerable<EmployeeInfo>> GetActiveAsync();
        
        #endregion
        
        #region Operaciones Específicas del Negocio
        
        /// <summary>
        /// Obtiene un empleado por número de documento
        /// </summary>
        Task<EmployeeInfo?> GetByDocumentNumberAsync(string documentNumber);
        
        /// <summary>
        /// Verifica si existe un empleado con el número de documento
        /// </summary>
        Task<bool> ExistsByDocumentNumberAsync(string documentNumber);
        
        /// <summary>
        /// Obtiene empleados con huella dactilar
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetWithFingerPrintAsync();
        
        /// <summary>
        /// Obtiene empleados por rango de edad
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetByAgeRangeAsync(int minAge, int maxAge);
        
        /// <summary>
        /// Obtiene empleados por rango de costo por hora
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetByCostPerHourRangeAsync(decimal minCost, decimal maxCost);
        
        #endregion
        
        #region Operaciones de Estadísticas y Reportes
        
        /// <summary>
        /// Obtiene estadísticas de empleados
        /// </summary>
        Task<EmployeeStatistics> GetStatisticsAsync();
        
        /// <summary>
        /// Obtiene empleados que no han registrado asistencia en un período
        /// </summary>
        Task<IEnumerable<EmployeeInfo>> GetWithoutAssistanceAsync(DateTime fromDate, DateTime toDate);
        
        /// <summary>
        /// Obtiene empleados con mayor tiempo trabajado en un período
        /// </summary>
        Task<IEnumerable<EmployeeWorkSummary>> GetTopWorkingAsync(DateTime fromDate, DateTime toDate, int top = 10);
        
        #endregion
        
        #region Operaciones de Validación
        
        /// <summary>
        /// Valida si un empleado puede ser creado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(CreateEmployeeDto createDto);
        
        /// <summary>
        /// Valida si un empleado puede ser actualizado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(int id, UpdateEmployeeDto updateDto);
        
        /// <summary>
        /// Valida si un empleado puede ser eliminado
        /// </summary>
        Task<(bool IsValid, List<string> Errors)> ValidateForDeletionAsync(int id);
        
        #endregion
    }
}
