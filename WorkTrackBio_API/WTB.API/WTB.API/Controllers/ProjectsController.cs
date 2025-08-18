using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Project;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de proyectos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(
            IProjectService projectService,
            ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener lista de proyectos con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de proyectos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetProjects([FromQuery] ProjectFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de proyectos con filtros");
                
                var (items, totalCount) = await _projectService.GetFilteredAsync(filter);
                
                // Convertir entidades a DTOs
                var projectDtos = items.Select(p => new ProjectResponseDto(
                    p.Id,
                    p.ProjectName,
                    p.StartDate,
                    p.EndDate,
                    p.StateId
                ));
                
                var response = APIResponse<IEnumerable<ProjectResponseDto>>.SuccessResponse(
                    projectDtos, 
                    $"Se encontraron {totalCount} proyectos"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de proyectos");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener proyecto por ID
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <returns>Proyecto encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<ProjectResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProjectById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyecto con ID: {Id}", id);
                
                var result = await _projectService.GetByIdAsync(id);
                
                if (result == null)
                {
                    return NotFound(APIResponse.ErrorResponse("Proyecto no encontrado", HttpStatusCode.NotFound));
                }
                
                var projectDto = new ProjectResponseDto(
                    result.Id,
                    result.ProjectName,
                    result.StartDate,
                    result.EndDate,
                    result.StateId
                );
                
                return Ok(APIResponse<ProjectResponseDto>.SuccessResponse(projectDto, "Proyecto obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyecto con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Crear nuevo proyecto
        /// </summary>
        /// <param name="dto">Datos del proyecto a crear</param>
        /// <returns>Proyecto creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<ProjectResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                _logger.LogInformation("Creando nuevo proyecto: {ProjectName}", dto.ProjectName);
                
                var result = await _projectService.CreateAsync(dto);
                
                var projectDto = new ProjectResponseDto(
                    result.Id,
                    result.ProjectName,
                    result.StartDate,
                    result.EndDate,
                    result.StateId
                );
                
                var response = APIResponse<ProjectResponseDto>.SuccessResponse(
                    projectDto, 
                    "Proyecto creado exitosamente"
                );
                
                return CreatedAtAction(nameof(GetProjectById), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proyecto: {ProjectName}", dto.ProjectName);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Actualizar proyecto existente
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Proyecto actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<ProjectResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto dto)
        {
            try
            {
                _logger.LogInformation("Actualizando proyecto con ID: {Id}", id);
                
                if (dto.Id != id)
                {
                    return BadRequest(APIResponse.ErrorResponse("El ID del proyecto no coincide con la URL"));
                }
                
                var result = await _projectService.UpdateAsync(id, dto);
                
                var projectDto = new ProjectResponseDto(
                    result.Id,
                    result.ProjectName,
                    result.StartDate,
                    result.EndDate,
                    result.StateId
                );
                
                var response = APIResponse<ProjectResponseDto>.SuccessResponse(
                    projectDto, 
                    "Proyecto actualizado exitosamente"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proyecto con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Asignar empleado a proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Confirmación de asignación</returns>
        [HttpPost("{projectId}/assign/{employeeId}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AssignEmployeeToProject(int projectId, int employeeId)
        {
            try
            {
                _logger.LogInformation("Asignando empleado {EmployeeId} al proyecto {ProjectId}", employeeId, projectId);
                
                await _projectService.AssignEmployeeAsync(projectId, employeeId, DateTime.UtcNow);
                
                var response = APIResponse.SuccessResponse("Empleado asignado al proyecto exitosamente");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar empleado {EmployeeId} al proyecto {ProjectId}", employeeId, projectId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Desasignar empleado de proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Confirmación de desasignación</returns>
        [HttpDelete("{projectId}/assign/{employeeId}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UnassignEmployeeFromProject(int projectId, int employeeId)
        {
            try
            {
                _logger.LogInformation("Desasignando empleado {EmployeeId} del proyecto {ProjectId}", employeeId, projectId);
                
                await _projectService.UnassignEmployeeAsync(projectId, employeeId);
                
                var response = APIResponse.SuccessResponse("Empleado desasignado del proyecto exitosamente");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasignar empleado {EmployeeId} del proyecto {ProjectId}", employeeId, projectId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Eliminar proyecto (soft delete)
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando proyecto con ID: {Id}", id);
                
                await _projectService.DeleteAsync(id);
                
                var response = APIResponse.SuccessResponse("Proyecto eliminado exitosamente");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proyecto con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener estadísticas de proyectos
        /// </summary>
        /// <returns>Estadísticas de proyectos</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProjectStatistics()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de proyectos");
                
                var statistics = await _projectService.GetStatisticsAsync();
                
                var response = APIResponse<object>.SuccessResponse(statistics, "Estadísticas obtenidas exitosamente");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de proyectos");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
