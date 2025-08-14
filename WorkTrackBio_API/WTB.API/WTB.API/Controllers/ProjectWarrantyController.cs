using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.ProjectWarranty;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de garantías de proyecto
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectWarrantyController : ControllerBase
    {
        private readonly IProjectWarrantyService _projectWarrantyService;
        private readonly ILogger<ProjectWarrantyController> _logger;

        public ProjectWarrantyController(
            IProjectWarrantyService projectWarrantyService,
            ILogger<ProjectWarrantyController> logger)
        {
            _projectWarrantyService = projectWarrantyService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene lista paginada de garantías de proyecto con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de garantías</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<PagedResponse<ProjectWarrantyResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProjectWarranties([FromQuery] ProjectWarrantyFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías de proyecto con filtros");

                var result = await _projectWarrantyService.GetFilteredAsync(filter);

                _logger.LogInformation("Garantías obtenidas: {Count} de {Total}", 
                    result.Data?.Count() ?? 0, result.TotalRecords);

                return Ok(APIResponse<PagedResponse<ProjectWarrantyResponseDto>>.SuccessResponse(result, 
                    "Garantías de proyecto obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías de proyecto");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene una garantía por ID
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <returns>Garantía si existe</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<ProjectWarrantyDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProjectWarranty(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantía con ID: {Id}", id);

                var warranty = await _projectWarrantyService.GetByIdAsync(id);
                if (warranty == null)
                {
                    _logger.LogWarning("Garantía con ID {Id} no encontrada", id);
                    return NotFound(APIResponse.ErrorResponse($"Garantía con ID {id} no encontrada", HttpStatusCode.NotFound));
                }

                _logger.LogInformation("Garantía obtenida: {Description}", warranty.WarrantyDescription);

                return Ok(APIResponse<ProjectWarrantyDetailsDto>.SuccessResponse(warranty, 
                    "Garantía obtenida exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantía con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Crea una nueva garantía de proyecto
        /// </summary>
        /// <param name="createDto">Datos para crear la garantía</param>
        /// <returns>Garantía creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<ProjectWarrantyResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateProjectWarranty([FromBody] CreateProjectWarrantyDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando garantía de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var warranty = await _projectWarrantyService.CreateAsync(createDto);

                _logger.LogInformation("Garantía creada exitosamente con ID: {Id}", warranty.Id);

                return CreatedAtAction(nameof(GetProjectWarranty), new { id = warranty.Id }, 
                    APIResponse<ProjectWarrantyResponseDto>.SuccessResponse(warranty, "Garantía de proyecto creada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al crear garantía: {Error}", ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando garantía de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Actualiza una garantía existente
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <param name="updateDto">Datos para actualizar</param>
        /// <returns>Garantía actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<ProjectWarrantyResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateProjectWarranty(int id, [FromBody] CreateProjectWarrantyDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando garantía con ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var warranty = await _projectWarrantyService.UpdateAsync(id, updateDto);

                _logger.LogInformation("Garantía actualizada exitosamente: {Description}", warranty.WarrantyDescription);

                return Ok(APIResponse<ProjectWarrantyResponseDto>.SuccessResponse(warranty, 
                    "Garantía actualizada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al actualizar garantía {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando garantía con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Elimina una garantía (soft delete)
        /// </summary>
        /// <param name="id">ID de la garantía</param>
        /// <returns>True si se eliminó correctamente</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProjectWarranty(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando garantía con ID: {Id}", id);

                var result = await _projectWarrantyService.DeleteAsync(id);

                _logger.LogInformation("Garantía eliminada exitosamente con ID: {Id}", id);

                return Ok(APIResponse<bool>.SuccessResponse(result, "Garantía eliminada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al eliminar garantía {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando garantía con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene garantías por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de garantías del proyecto</returns>
        [HttpGet("by-project/{projectId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectWarrantyListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWarrantiesByProject(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para proyecto ID: {ProjectId}", projectId);

                var warranties = await _projectWarrantyService.GetByProjectAsync(projectId);

                _logger.LogInformation("Garantías por proyecto obtenidas: {Count}", warranties.Count());

                return Ok(APIResponse<IEnumerable<ProjectWarrantyListDto>>.SuccessResponse(warranties, 
                    "Garantías por proyecto obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías para proyecto ID: {ProjectId}", projectId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene garantías por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de garantías registradas por el empleado</returns>
        [HttpGet("by-employee/{employeeId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectWarrantyListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWarrantiesByEmployee(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para empleado ID: {EmployeeId}", employeeId);

                var warranties = await _projectWarrantyService.GetByEmployeeAsync(employeeId);

                _logger.LogInformation("Garantías por empleado obtenidas: {Count}", warranties.Count());

                return Ok(APIResponse<IEnumerable<ProjectWarrantyListDto>>.SuccessResponse(warranties, 
                    "Garantías por empleado obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías para empleado ID: {EmployeeId}", employeeId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene garantías por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de garantías con el estado especificado</returns>
        [HttpGet("by-state/{stateId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectWarrantyListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWarrantiesByState(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para estado ID: {StateId}", stateId);

                var warranties = await _projectWarrantyService.GetByStateAsync(stateId);

                _logger.LogInformation("Garantías por estado obtenidas: {Count}", warranties.Count());

                return Ok(APIResponse<IEnumerable<ProjectWarrantyListDto>>.SuccessResponse(warranties, 
                    "Garantías por estado obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías para estado ID: {StateId}", stateId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene garantías por rango de fechas
        /// </summary>
        /// <param name="fromDate">Fecha desde</param>
        /// <param name="toDate">Fecha hasta</param>
        /// <returns>Lista de garantías en el rango de fechas</returns>
        [HttpGet("by-date-range")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectWarrantyListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWarrantiesByDateRange([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                if (fromDate > toDate)
                {
                    return BadRequest(APIResponse.ErrorResponse("La fecha desde debe ser menor o igual a la fecha hasta"));
                }

                _logger.LogInformation("Obteniendo garantías entre fechas: {FromDate} - {ToDate}", fromDate, toDate);

                var warranties = await _projectWarrantyService.GetByDateRangeAsync(fromDate, toDate);

                _logger.LogInformation("Garantías por rango de fechas obtenidas: {Count}", warranties.Count());

                return Ok(APIResponse<IEnumerable<ProjectWarrantyListDto>>.SuccessResponse(warranties, 
                    "Garantías por rango de fechas obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene garantías por rango de costo
        /// </summary>
        /// <param name="minCost">Costo mínimo</param>
        /// <param name="maxCost">Costo máximo</param>
        /// <returns>Lista de garantías en el rango de costos</returns>
        [HttpGet("by-cost-range")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectWarrantyListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWarrantiesByCostRange([FromQuery] decimal minCost, [FromQuery] decimal maxCost)
        {
            try
            {
                if (minCost < 0 || maxCost < 0 || minCost > maxCost)
                {
                    return BadRequest(APIResponse.ErrorResponse("Los costos deben ser valores positivos y el costo mínimo debe ser menor o igual al máximo"));
                }

                _logger.LogInformation("Obteniendo garantías entre costos: {MinCost} - {MaxCost}", minCost, maxCost);

                var warranties = await _projectWarrantyService.GetByCostRangeAsync(minCost, maxCost);

                _logger.LogInformation("Garantías por rango de costos obtenidas: {Count}", warranties.Count());

                return Ok(APIResponse<IEnumerable<ProjectWarrantyListDto>>.SuccessResponse(warranties, 
                    "Garantías por rango de costos obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías por rango de costos: {MinCost} - {MaxCost}", minCost, maxCost);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Busca garantías por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de garantías que coinciden</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProjectWarrantyListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchWarranties([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(APIResponse.ErrorResponse("El término de búsqueda es obligatorio"));
                }

                _logger.LogInformation("Buscando garantías con término: {SearchTerm}", searchTerm);

                var warranties = await _projectWarrantyService.SearchAsync(searchTerm);

                _logger.LogInformation("Búsqueda completada: {Count} garantías encontradas", warranties.Count());

                return Ok(APIResponse<IEnumerable<ProjectWarrantyListDto>>.SuccessResponse(warranties, 
                    "Búsqueda completada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando garantías con término: {SearchTerm}", searchTerm);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene estadísticas de garantías
        /// </summary>
        /// <returns>Estadísticas de garantías</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetWarrantyStatistics()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de garantías");

                var statistics = await _projectWarrantyService.GetStatisticsAsync();

                _logger.LogInformation("Estadísticas obtenidas exitosamente");

                return Ok(APIResponse<object>.SuccessResponse(statistics, "Estadísticas obtenidas exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de garantías");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
