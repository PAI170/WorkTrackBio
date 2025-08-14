using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar asignaciones de empleados a proyectos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectAssignController : ControllerBase
    {
        private readonly IProjectAssignService _projectAssignService;
        private readonly ILogger<ProjectAssignController> _logger;

        public ProjectAssignController(
            IProjectAssignService projectAssignService,
            ILogger<ProjectAssignController> logger)
        {
            _projectAssignService = projectAssignService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene asignaciones filtradas con paginación
        /// </summary>
        /// <param name="filter">Filtros de búsqueda y paginación</param>
        /// <returns>Lista paginada de asignaciones</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ProjectAssignListDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResponse<ProjectAssignListDto>>> GetProjectAssigns([FromQuery] ProjectAssignFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo asignaciones filtradas. Página: {Page}, Tamaño: {PageSize}", filter.Page, filter.PageSize);
                
                var result = await _projectAssignService.GetFilteredAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones filtradas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una asignación por ID
        /// </summary>
        /// <param name="id">ID de la asignación</param>
        /// <returns>Detalles de la asignación</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectAssignDetailsDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectAssignDetailsDto>> GetProjectAssign(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo asignación con ID: {Id}", id);
                
                var result = await _projectAssignService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound($"Asignación con ID {id} no encontrada");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignación con ID: {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva asignación
        /// </summary>
        /// <param name="createDto">Datos para crear la asignación</param>
        /// <returns>Asignación creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectAssignResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectAssignResponseDto>> CreateProjectAssign(CreateProjectAssignDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando nueva asignación para empleado {EmployeeId} en proyecto {ProjectId}", 
                    createDto.EmployeeId, createDto.ProjectId);
                
                var result = await _projectAssignService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetProjectAssign), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al crear asignación: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando asignación");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una asignación existente
        /// </summary>
        /// <param name="id">ID de la asignación</param>
        /// <param name="updateDto">Datos para actualizar la asignación</param>
        /// <returns>Asignación actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectAssignResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectAssignResponseDto>> UpdateProjectAssign(int id, UpdateProjectAssignDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest("El ID de la URL no coincide con el ID del DTO");
                }

                _logger.LogInformation("Actualizando asignación con ID: {Id}", id);
                
                var result = await _projectAssignService.UpdateAsync(updateDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al actualizar asignación con ID {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando asignación con ID: {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una asignación
        /// </summary>
        /// <param name="id">ID de la asignación</param>
        /// <returns>Resultado de la eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> DeleteProjectAssign(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando asignación con ID: {Id}", id);
                
                var result = await _projectAssignService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Asignación con ID {id} no encontrada");
                }

                return Ok(new { message = "Asignación eliminada exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al eliminar asignación con ID {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando asignación con ID: {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene asignaciones por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de asignaciones del empleado</returns>
        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(typeof(IEnumerable<ProjectAssignResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProjectAssignResponseDto>>> GetByEmployee(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo asignaciones del empleado con ID: {EmployeeId}", employeeId);
                
                var result = await _projectAssignService.GetByEmployeeAsync(employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones del empleado con ID: {EmployeeId}", employeeId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene asignaciones por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de asignaciones del proyecto</returns>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(IEnumerable<ProjectAssignResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProjectAssignResponseDto>>> GetByProject(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo asignaciones del proyecto con ID: {ProjectId}", projectId);
                
                var result = await _projectAssignService.GetByProjectAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones del proyecto con ID: {ProjectId}", projectId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene asignaciones activas
        /// </summary>
        /// <returns>Lista de asignaciones activas</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<ProjectAssignResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProjectAssignResponseDto>>> GetActive()
        {
            try
            {
                _logger.LogInformation("Obteniendo asignaciones activas");
                
                var result = await _projectAssignService.GetActiveAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones activas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene asignaciones por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha de inicio</param>
        /// <param name="toDate">Fecha de fin</param>
        /// <returns>Lista de asignaciones en el rango de fechas</returns>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(IEnumerable<ProjectAssignResponseDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProjectAssignResponseDto>>> GetByDateRange(
            [FromQuery] DateTime fromDate, 
            [FromQuery] DateTime toDate)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest("La fecha de inicio no puede ser posterior a la fecha de fin");
                }

                _logger.LogInformation("Obteniendo asignaciones por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                
                var result = await _projectAssignService.GetByDateRangeAsync(fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene asignaciones por estado del proyecto
        /// </summary>
        /// <param name="projectState">Estado del proyecto</param>
        /// <returns>Lista de asignaciones con el estado del proyecto especificado</returns>
        [HttpGet("project-state/{projectState}")]
        [ProducesResponseType(typeof(IEnumerable<ProjectAssignResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProjectAssignResponseDto>>> GetByProjectState(string projectState)
        {
            try
            {
                _logger.LogInformation("Obteniendo asignaciones por estado del proyecto: {ProjectState}", projectState);
                
                var result = await _projectAssignService.GetByProjectStateAsync(projectState);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por estado del proyecto: {ProjectState}", projectState);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene asignaciones por estado del empleado
        /// </summary>
        /// <param name="employeeState">Estado del empleado</param>
        /// <returns>Lista de asignaciones con el estado del empleado especificado</returns>
        [HttpGet("employee-state/{employeeState}")]
        [ProducesResponseType(typeof(IEnumerable<ProjectAssignResponseDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<ProjectAssignResponseDto>>> GetByEmployeeState(string employeeState)
        {
            try
            {
                _logger.LogInformation("Obteniendo asignaciones por estado del empleado: {EmployeeState}", employeeState);
                
                var result = await _projectAssignService.GetByEmployeeStateAsync(employeeState);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por estado del empleado: {EmployeeState}", employeeState);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Verifica si un empleado está asignado a un proyecto
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="fromDate">Fecha de inicio opcional</param>
        /// <param name="toDate">Fecha de fin opcional</param>
        /// <returns>True si está asignado, false en caso contrario</returns>
        [HttpGet("check-assignment")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<bool>> CheckAssignment(
            [FromQuery] int employeeId, 
            [FromQuery] int projectId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                _logger.LogInformation("Verificando si empleado {EmployeeId} está asignado al proyecto {ProjectId}", employeeId, projectId);
                
                var result = await _projectAssignService.IsEmployeeAssignedToProjectAsync(employeeId, projectId, fromDate, toDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando asignación del empleado {EmployeeId} al proyecto {ProjectId}", employeeId, projectId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de asignaciones
        /// </summary>
        /// <returns>Estadísticas de asignaciones</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ProjectAssignStatsDto), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectAssignStatsDto>> GetStatistics()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de asignaciones");
                
                var result = await _projectAssignService.GetStatisticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de asignaciones");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Finaliza una asignación estableciendo fecha de fin
        /// </summary>
        /// <param name="id">ID de la asignación</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Asignación finalizada</returns>
        [HttpPost("{id}/end")]
        [ProducesResponseType(typeof(ProjectAssignResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectAssignResponseDto>> EndAssignment(int id, [FromBody] DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Finalizando asignación con ID: {Id}, fecha de fin: {EndDate}", id, endDate);
                
                var result = await _projectAssignService.EndAssignmentAsync(id, endDate);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al finalizar asignación con ID {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizando asignación con ID: {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Reactiva una asignación removiendo fecha de fin
        /// </summary>
        /// <param name="id">ID de la asignación</param>
        /// <returns>Asignación reactivada</returns>
        [HttpPost("{id}/reactivate")]
        [ProducesResponseType(typeof(ProjectAssignResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ProjectAssignResponseDto>> ReactivateAssignment(int id)
        {
            try
            {
                _logger.LogInformation("Reactivando asignación con ID: {Id}", id);
                
                var result = await _projectAssignService.ReactivateAssignmentAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al reactivar asignación con ID {Id}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reactivando asignación con ID: {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
