using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectWarrantyRepository
{
    /// <summary>
    /// Interfaz para el repositorio de ProjectWarranty
    /// </summary>
    public interface IProjectWarrantyRepository
    {
        /// <summary>
        /// Obtiene todas las garantías de proyectos
        /// </summary>
        Task<IEnumerable<ProjectWarranty>> GetAllAsync();

        /// <summary>
        /// Obtiene una garantía de proyecto por ID
        /// </summary>
        Task<ProjectWarranty?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene garantías de proyectos por proyecto
        /// </summary>
        Task<IEnumerable<ProjectWarranty>> GetByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene garantías de proyectos por empleado
        /// </summary>
        Task<IEnumerable<ProjectWarranty>> GetByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene garantías de proyectos por estado
        /// </summary>
        Task<IEnumerable<ProjectWarranty>> GetByStateAsync(int stateId);

        /// <summary>
        /// Obtiene garantías de proyectos por rango de fechas
        /// </summary>
        Task<IEnumerable<ProjectWarranty>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene garantías de proyectos por proyecto y estado
        /// </summary>
        Task<IEnumerable<ProjectWarranty>> GetByProjectAndStateAsync(int projectId, int stateId);

        /// <summary>
        /// Crea una nueva garantía de proyecto
        /// </summary>
        Task<ProjectWarranty> CreateAsync(ProjectWarranty projectWarranty);

        /// <summary>
        /// Actualiza una garantía de proyecto existente
        /// </summary>
        Task<ProjectWarranty> UpdateAsync(ProjectWarranty projectWarranty);

        /// <summary>
        /// Elimina una garantía de proyecto
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica si una garantía de proyecto tiene dependencias
        /// </summary>
        Task<bool> HasDependenciesAsync(int projectWarrantyId);

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
