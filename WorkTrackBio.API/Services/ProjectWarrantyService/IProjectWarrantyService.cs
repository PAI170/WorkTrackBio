using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;

namespace WorkTrackBio.API.Services.ProjectWarrantyService
{
    /// <summary>
    /// Interfaz para el servicio de ProjectWarranty
    /// </summary>
    public interface IProjectWarrantyService
    {
        /// <summary>
        /// Obtiene todas las garantías de proyectos
        /// </summary>
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetAllProjectWarrantiesAsync();

        /// <summary>
        /// Obtiene una garantía de proyecto por ID
        /// </summary>
        Task<ApiResponse<ProjectWarrantyDataTransferObject>> GetProjectWarrantyByIdAsync(int id);

        /// <summary>
        /// Obtiene garantías de proyectos por proyecto
        /// </summary>
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAsync(int projectId);

        /// <summary>
        /// Obtiene garantías de proyectos por empleado
        /// </summary>
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByEmployeeAsync(int employeeId);

        /// <summary>
        /// Obtiene garantías de proyectos por estado
        /// </summary>
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByStateAsync(int stateId);

        /// <summary>
        /// Obtiene garantías de proyectos por rango de fechas
        /// </summary>
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene garantías de proyectos por proyecto y estado
        /// </summary>
        Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAndStateAsync(int projectId, int stateId);

        /// <summary>
        /// Crea una nueva garantía de proyecto
        /// </summary>
        Task<ApiResponse<ProjectWarrantyDataTransferObject>> CreateProjectWarrantyAsync(CreateProjectWarrantyDataTransferObject createDto);

        /// <summary>
        /// Actualiza una garantía de proyecto existente
        /// </summary>
        Task<ApiResponse<ProjectWarrantyDataTransferObject>> UpdateProjectWarrantyAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto);

        /// <summary>
        /// Elimina una garantía de proyecto
        /// </summary>
        Task<ApiResponse<bool>> DeleteProjectWarrantyAsync(int id);
    }
}
