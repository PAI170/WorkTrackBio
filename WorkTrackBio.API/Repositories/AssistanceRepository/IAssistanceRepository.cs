using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.AssistanceRepository
{
    /// <summary>
    /// Interfaz para el repositorio de Assistance
    /// </summary>
    public interface IAssistanceRepository
    {
        /// <summary>
        /// Obtiene todos los registros de asistencia
        /// </summary>
        Task<IEnumerable<Assistance>> GetAllAsync();

        /// <summary>
        /// Obtiene un registro de asistencia por ID
        /// </summary>
        Task<Assistance?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene registros de asistencia por empleado
        /// </summary>
        Task<IEnumerable<Assistance>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene registros de asistencia por proyecto
        /// </summary>
        Task<IEnumerable<Assistance>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene registros de asistencia por fecha
        /// </summary>
        Task<IEnumerable<Assistance>> GetByDateAsync(DateOnly date);

        /// <summary>
        /// Obtiene registros de asistencia por rango de fechas
        /// </summary>
        Task<IEnumerable<Assistance>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Obtiene registros de asistencia por empleado y proyecto
        /// </summary>
        Task<IEnumerable<Assistance>> GetByEmployeeAndProjectAsync(int employeeId, int projectId);

        /// <summary>
        /// Obtiene registros de asistencia por empleado y rango de fechas
        /// </summary>
        Task<IEnumerable<Assistance>> GetByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Obtiene registros de asistencia por proyecto y rango de fechas
        /// </summary>
        Task<IEnumerable<Assistance>> GetByProjectAndDateRangeAsync(int projectId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Obtiene registros de asistencia por proyecto y empleado específico
        /// </summary>
        Task<IEnumerable<Assistance>> GetByProjectAndEmployeeAsync(int projectId, int employeeId);

        /// <summary>
        /// Verifica si existe un registro de asistencia con CheckIn abierto para un empleado
        /// </summary>
        Task<bool> HasOpenCheckInAsync(int employeeId);

        /// <summary>
        /// Crea un nuevo registro de asistencia
        /// </summary>
        Task<Assistance> CreateAsync(Assistance assistance);

        /// <summary>
        /// Actualiza un registro de asistencia existente
        /// </summary>
        Task<Assistance> UpdateAsync(Assistance assistance);

        /// <summary>
        /// Elimina un registro de asistencia
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica si un registro de asistencia tiene dependencias
        /// </summary>
        Task<bool> HasDependenciesAsync(int assistanceId);

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
