using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Assistance;
using WorkTrackBio.API.Services.AssistanceService;
using WorkTrackBio.API.Validators.AssistanceValidator;

namespace WorkTrackBio.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de registros de asistencia
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AssistanceController : ControllerBase
    {
        private readonly IAssistanceService _assistanceService;
        private readonly IAssistanceValidator _assistanceValidator;

        public AssistanceController(
            IAssistanceService assistanceService,
            IAssistanceValidator assistanceValidator)
        {
            _assistanceService = assistanceService ?? throw new ArgumentNullException(nameof(assistanceService));
            _assistanceValidator = assistanceValidator ?? throw new ArgumentNullException(nameof(assistanceValidator));
        }

        /// <summary>
        /// Obtiene todos los registros de asistencia
        /// </summary>
        /// <returns>Lista de registros de asistencia</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAllAssistances()
        {
            var response = await _assistanceService.GetAllAssistancesAsync();
            return Ok(response);
        }

        /// <summary>
        /// Obtiene un registro de asistencia por ID
        /// </summary>
        /// <param name="id">ID del registro de asistencia</param>
        /// <returns>Registro de asistencia</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<AssistanceDataTransferObject>>> GetAssistanceById(int id)
        {
            var response = await _assistanceService.GetAssistanceByIdAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("No se encontró"))
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por empleado
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de registros de asistencia del empleado</returns>
        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByEmployee(int employeeId)
        {
            var response = await _assistanceService.GetAssistancesByEmployeeAsync(employeeId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de registros de asistencia del proyecto</returns>
        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByProject(int projectId)
        {
            var response = await _assistanceService.GetAssistancesByProjectAsync(projectId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por fecha
        /// </summary>
        /// <param name="date">Fecha en formato yyyy-MM-dd</param>
        /// <returns>Lista de registros de asistencia de la fecha</returns>
        [HttpGet("date/{date:datetime}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByDate(DateTime date)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            var response = await _assistanceService.GetAssistancesByDateAsync(dateOnly);
            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio en formato yyyy-MM-dd</param>
        /// <param name="endDate">Fecha de fin en formato yyyy-MM-dd</param>
        /// <returns>Lista de registros de asistencia del rango de fechas</returns>
        [HttpGet("daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);
            
            var response = await _assistanceService.GetAssistancesByDateRangeAsync(startDateOnly, endDateOnly);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por empleado y proyecto
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de registros de asistencia del empleado en el proyecto</returns>
        [HttpGet("employee/{employeeId}/project/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByEmployeeAndProject(int employeeId, int projectId)
        {
            var response = await _assistanceService.GetAssistancesByEmployeeAndProjectAsync(employeeId, projectId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por empleado y rango de fechas
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <param name="startDate">Fecha de inicio en formato yyyy-MM-dd</param>
        /// <param name="endDate">Fecha de fin en formato yyyy-MM-dd</param>
        /// <returns>Lista de registros de asistencia del empleado en el rango de fechas</returns>
        [HttpGet("employee/{employeeId}/daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByEmployeeAndDateRange(
            int employeeId,
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);
            
            var response = await _assistanceService.GetAssistancesByEmployeeAndDateRangeAsync(employeeId, startDateOnly, endDateOnly);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por proyecto y rango de fechas
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="startDate">Fecha de inicio en formato yyyy-MM-dd</param>
        /// <param name="endDate">Fecha de fin en formato yyyy-MM-dd</param>
        /// <returns>Lista de registros de asistencia del proyecto en el rango de fechas</returns>
        [HttpGet("project/{projectId}/daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByProjectAndDateRange(
            int projectId,
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);
            
            var response = await _assistanceService.GetAssistancesByProjectAndDateRangeAsync(projectId, startDateOnly, endDateOnly);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Obtiene registros de asistencia por proyecto y empleado específico
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <param name="employeeId">ID del empleado</param>
        /// <returns>Lista de registros de asistencia del proyecto y empleado específico</returns>
        [HttpGet("project/{projectId}/employee/{employeeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByProjectAndEmployee(
            int projectId, 
            int employeeId)
        {
            var response = await _assistanceService.GetAssistancesByProjectAndEmployeeAsync(projectId, employeeId);
            
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Crea un nuevo registro de asistencia
        /// </summary>
        /// <param name="createDto">Datos para crear el registro de asistencia</param>
        /// <returns>Registro de asistencia creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 201)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<AssistanceDataTransferObject>>> CreateAssistance(CreateAssistanceDataTransferObject createDto)
        {
            var response = await _assistanceService.CreateAssistanceAsync(createDto);
            
            if (!response.Success)
                return BadRequest(response);

            return CreatedAtAction(nameof(GetAssistanceById), new { id = response.Data!.Id }, response);
        }

        /// <summary>
        /// Actualiza un registro de asistencia existente
        /// </summary>
        /// <param name="id">ID del registro de asistencia</param>
        /// <param name="updateDto">Datos para actualizar el registro de asistencia</param>
        /// <returns>Registro de asistencia actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<AssistanceDataTransferObject>>> UpdateAssistance(int id, UpdateAssistanceDataTransferObject updateDto)
        {
            var response = await _assistanceService.UpdateAssistanceAsync(id, updateDto);
            
            if (!response.Success)
            {
                if (response.Message.Contains("No se encontró"))
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Elimina un registro de asistencia
        /// </summary>
        /// <param name="id">ID del registro de asistencia</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAssistance(int id)
        {
            var response = await _assistanceService.DeleteAssistanceAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("No se encontró"))
                    return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Calcula las horas totales de un registro de asistencia
        /// </summary>
        /// <param name="id">ID del registro de asistencia</param>
        /// <returns>Horas totales calculadas</returns>
        [HttpGet("{id}/calculate-hours")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 200)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 400)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 404)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 500)]
        public async Task<ActionResult<ApiResponse<decimal>>> CalculateTotalHours(int id)
        {
            var response = await _assistanceService.CalculateTotalHoursAsync(id);
            
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
