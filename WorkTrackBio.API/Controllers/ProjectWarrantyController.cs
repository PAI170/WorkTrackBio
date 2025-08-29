using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;
using WorkTrackBio.API.Services.ProjectWarrantyService;

namespace WorkTrackBio.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de garantías de proyectos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectWarrantyController : ControllerBase
    {
        private readonly IProjectWarrantyService _projectWarrantyService;

        public ProjectWarrantyController(IProjectWarrantyService projectWarrantyService)
        {
            _projectWarrantyService = projectWarrantyService ?? throw new ArgumentNullException(nameof(projectWarrantyService));
        }

        /// <summary>
        /// Obtiene todas las garantías de proyectos
        /// </summary>
        /// <returns>Lista de garantías de proyectos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetAllProjectWarranties()
        {
            var response = await _projectWarrantyService.GetAllProjectWarrantiesAsync();
            return Ok(response);
        }

        /// <summary>
        /// Obtiene una garantía de proyecto por ID
        /// </summary>
        /// <param name="id">ID de la garantía de proyecto</param>
        /// <returns>Garantía de proyecto</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<ProjectWarrantyDataTransferObject>>> GetProjectWarrantyById(int id)
        {
            var response = await _projectWarrantyService.GetProjectWarrantyByIdAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("No se encontró"))
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene garantías de proyectos por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de garantías del proyecto</returns>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByProject(int projectId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByProjectAsync(projectId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene garantías de proyectos por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de garantías del empleado</returns>
        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByEmployee(int employeeId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByEmployeeAsync(employeeId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene garantías de proyectos por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de garantías del estado</returns>
        [HttpGet("state/{stateId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByState(int stateId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByStateAsync(stateId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene garantías de proyectos por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio en formato yyyy-MM-dd</param>
        /// <param name="endDate">Fecha de fin en formato yyyy-MM-dd</param>
        /// <returns>Lista de garantías del rango de fechas</returns>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByDateRangeAsync(startDate, endDate);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene garantías de proyectos por proyecto y estado
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de garantías del proyecto y estado</returns>
        [HttpGet("project/{projectId}/state/{stateId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByProjectAndState(
            int projectId, 
            int stateId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByProjectAndStateAsync(projectId, stateId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Crea una nueva garantía de proyecto
        /// </summary>
        /// <param name="createDto">Datos para crear la garantía de proyecto</param>
        /// <returns>Garantía de proyecto creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 201)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<ProjectWarrantyDataTransferObject>>> CreateProjectWarranty(CreateProjectWarrantyDataTransferObject createDto)
        {
            var response = await _projectWarrantyService.CreateProjectWarrantyAsync(createDto);
            
            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetProjectWarrantyById), new { id = response.Data!.Id }, response);
        }

        /// <summary>
        /// Actualiza una garantía de proyecto existente
        /// </summary>
        /// <param name="id">ID de la garantía de proyecto</param>
        /// <param name="updateDto">Datos para actualizar la garantía de proyecto</param>
        /// <returns>Garantía de proyecto actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<ProjectWarrantyDataTransferObject>>> UpdateProjectWarranty(int id, UpdateProjectWarrantyDataTransferObject updateDto)
        {
            var response = await _projectWarrantyService.UpdateProjectWarrantyAsync(id, updateDto);
            
            if (!response.Success)
            {
                if (response.Message.Contains("No se encontró"))
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Elimina una garantía de proyecto
        /// </summary>
        /// <param name="id">ID de la garantía de proyecto</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProjectWarranty(int id)
        {
            var response = await _projectWarrantyService.DeleteProjectWarrantyAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("No se encontró"))
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
