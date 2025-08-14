using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.ProjectMaintenance;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de mantenimientos de proyecto
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectMaintenanceController : ControllerBase
    {
        private readonly IProjectMaintenanceService _projectMaintenanceService;
        private readonly ILogger<ProjectMaintenanceController> _logger;

        public ProjectMaintenanceController(
            IProjectMaintenanceService projectMaintenanceService,
            ILogger<ProjectMaintenanceController> logger)
        {
            _projectMaintenanceService = projectMaintenanceService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene lista paginada de mantenimientos de proyecto con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de mantenimientos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<PagedResponse<ProjectMaintenanceResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProjectMaintenances([FromQuery] ProjectMaintenanceFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos de proyecto con filtros");

                var result = await _projectMaintenanceService.GetFilteredAsync(filter);

                _logger.LogInformation("Mantenimientos obtenidos: {Count} de {Total}", 
                    result.Data?.Count() ?? 0, result.TotalRecords);

                return Ok(APIResponse<PagedResponse<ProjectMaintenanceResponseDto>>.SuccessResponse(result, 
                    "Mantenimientos de proyecto obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos de proyecto");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene un mantenimiento por ID
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <returns>Mantenimiento si existe</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<ProjectMaintenanceDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProjectMaintenance(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimiento con ID: {Id}", id);

                var maintenance = await _projectMaintenanceService.GetByIdAsync(id);
                if (maintenance == null)
                {
                    _logger.LogWarning("Mantenimiento con ID {Id} no encontrado", id);
                    return NotFound(APIResponse.ErrorResponse($"Mantenimiento con ID {id} no encontrado", HttpStatusCode.NotFound));
                }

                _logger.LogInformation("Mantenimiento obtenido: {Description}", maintenance.MaintenanceDescription);

                return Ok(APIResponse<ProjectMaintenanceDetailsDto>.SuccessResponse(maintenance, 
                    "Mantenimiento obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimiento con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Crea un nuevo mantenimiento de proyecto
        /// </summary>
        /// <param name="createDto">Datos para crear el mantenimiento</param>
        /// <returns>Mantenimiento creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<ProjectMaintenanceResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateProjectMaintenance([FromBody] CreateProjectMaintenanceDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando mantenimiento de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var maintenance = await _projectMaintenanceService.CreateAsync(createDto);

                _logger.LogInformation("Mantenimiento creado exitosamente con ID: {Id}", maintenance.Id);

                return CreatedAtAction(nameof(GetProjectMaintenance), new { id = maintenance.Id }, 
                    APIResponse<ProjectMaintenanceResponseDto>.SuccessResponse(maintenance, "Mantenimiento de proyecto creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al crear mantenimiento: {Error}", ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando mantenimiento de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Actualiza un mantenimiento existente
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <param name="updateDto">Datos para actualizar</param>
        /// <returns>Mantenimiento actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<ProjectMaintenanceResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateProjectMaintenance(int id, [FromBody] CreateProjectMaintenanceDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando mantenimiento con ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var maintenance = await _projectMaintenanceService.UpdateAsync(id, updateDto);

                _logger.LogInformation("Mantenimiento actualizado exitosamente: {Description}", maintenance.MaintenanceDescription);

                return Ok(APIResponse<ProjectMaintenanceResponseDto>.SuccessResponse(maintenance, 
                    "Mantenimiento actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al actualizar mantenimiento {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando mantenimiento con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Elimina un mantenimiento (soft delete)
        /// </summary>
        /// <param name="id">ID del mantenimiento</param>
        /// <returns>True si se eliminó correctamente</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProjectMaintenance(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando mantenimiento con ID: {Id}", id);

                var result = await _projectMaintenanceService.DeleteAsync(id);

                _logger.LogInformation("Mantenimiento eliminado exitosamente con ID: {Id}", id);

                return Ok(APIResponse<bool>.SuccessResponse(result, "Mantenimiento eliminado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al eliminar mantenimiento {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando mantenimiento con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de mantenimientos del proyecto</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectMaintenanceListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMaintenancesByProject(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para proyecto ID: {ProjectId}", projectId);

                var maintenances = await _projectMaintenanceService.GetByProjectAsync(projectId);

                _logger.LogInformation("Mantenimientos por proyecto obtenidos: {Count}", maintenances.Count());

                return Ok(APIResponse<IEnumerable<ProjectMaintenanceListDto>>.SuccessResponse(maintenances, 
                    "Mantenimientos por proyecto obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos para proyecto ID: {ProjectId}", projectId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de mantenimientos realizados por el empleado</returns>
        [HttpGet("by-employee/{employeeId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectMaintenanceListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMaintenancesByEmployee(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para empleado ID: {EmployeeId}", employeeId);

                var maintenances = await _projectMaintenanceService.GetByEmployeeAsync(employeeId);

                _logger.LogInformation("Mantenimientos por empleado obtenidos: {Count}", maintenances.Count());

                return Ok(APIResponse<IEnumerable<ProjectMaintenanceListDto>>.SuccessResponse(maintenances, 
                    "Mantenimientos por empleado obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos para empleado ID: {EmployeeId}", employeeId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de mantenimientos con el estado especificado</returns>
        [HttpGet("by-state/{stateId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectMaintenanceListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMaintenancesByState(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para estado ID: {StateId}", stateId);

                var maintenances = await _projectMaintenanceService.GetByStateAsync(stateId);

                _logger.LogInformation("Mantenimientos por estado obtenidos: {Count}", maintenances.Count());

                return Ok(APIResponse<IEnumerable<ProjectMaintenanceListDto>>.SuccessResponse(maintenances, 
                    "Mantenimientos por estado obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos para estado ID: {StateId}", stateId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de mantenimientos en el rango de fechas</returns>
        [HttpGet("by-date-range")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectMaintenanceListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMaintenancesByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest(APIResponse.ErrorResponse("La fecha desde debe ser menor o igual a la fecha hasta"));
                }

                _logger.LogInformation("Obteniendo mantenimientos entre fechas: {FromDate} - {ToDate}", fromDate, toDate);

                var maintenances = await _projectMaintenanceService.GetByDateRangeAsync(fromDate, toDate);

                _logger.LogInformation("Mantenimientos por rango de fechas obtenidos: {Count}", maintenances.Count());

                return Ok(APIResponse<IEnumerable<ProjectMaintenanceListDto>>.SuccessResponse(maintenances, 
                    "Mantenimientos por rango de fechas obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por rango de costo
        /// </summary>
        /// <param name="minCost">Costo mínimo</param>
        /// <param name="maxCost">Costo máximo</param>
        /// <returns>Lista de mantenimientos en el rango de costos</returns>
        [HttpGet("by-cost-range")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectMaintenanceListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMaintenancesByCostRange([FromQuery] decimal minCost, [FromQuery] decimal maxCost)
        {
            try
            {
                if (minCost < 0 || maxCost < 0 || minCost > maxCost)
                {
                    return BadRequest(APIResponse.ErrorResponse("Los costos deben ser valores positivos y el costo mínimo debe ser menor o igual al máximo"));
                }

                _logger.LogInformation("Obteniendo mantenimientos entre costos: {MinCost} - {MaxCost}", minCost, maxCost);

                var maintenances = await _projectMaintenanceService.GetByCostRangeAsync(minCost, maxCost);

                _logger.LogInformation("Mantenimientos por rango de costos obtenidos: {Count}", maintenances.Count());

                return Ok(APIResponse<IEnumerable<ProjectMaintenanceListDto>>.SuccessResponse(maintenances, 
                    "Mantenimientos por rango de costos obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos por rango de costos: {MinCost} - {MaxCost}", minCost, maxCost);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Busca mantenimientos por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de mantenimientos que coinciden</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectMaintenanceListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchMaintenances([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(APIResponse.ErrorResponse("El término de búsqueda es obligatorio"));
                }

                _logger.LogInformation("Buscando mantenimientos con término: {SearchTerm}", searchTerm);

                var maintenances = await _projectMaintenanceService.SearchAsync(searchTerm);

                _logger.LogInformation("Búsqueda completada: {Count} mantenimientos encontrados", maintenances.Count());

                return Ok(APIResponse<IEnumerable<ProjectMaintenanceListDto>>.SuccessResponse(maintenances, 
                    "Búsqueda completada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando mantenimientos con término: {SearchTerm}", searchTerm);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene estadísticas de mantenimientos
        /// </summary>
        /// <returns>Estadísticas de mantenimientos</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetMaintenanceStatistics()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de mantenimientos");

                var statistics = await _projectMaintenanceService.GetStatisticsAsync();

                _logger.LogInformation("Estadísticas obtenidas exitosamente");

                return Ok(APIResponse<object>.SuccessResponse(statistics, "Estadísticas obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de mantenimientos");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
