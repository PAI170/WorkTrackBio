using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de asistencias
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AssistanceController : ControllerBase
    {
        private readonly IAssistanceService _assistanceService;
        private readonly ILogger<AssistanceController> _logger;

        public AssistanceController(
            IAssistanceService assistanceService,
            ILogger<AssistanceController> logger)
        {
            _assistanceService = assistanceService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener lista de asistencias con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de asistencias</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<AssistanceResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAssistances([FromQuery] AssistanceFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de asistencias con filtros");
                
                var (items, totalCount) = await _assistanceService.GetFilteredAsync(filter);
                
                // Convertir entidades a DTOs
                var assistanceDtos = items.Select(a => new AssistanceResponseDto(
                    a.Id,
                    a.EmployeeId,
                    a.Employee?.FirstName + " " + a.Employee?.LastName ?? "Empleado no encontrado",
                    a.ProjectId,
                    a.Project?.ProjectName ?? "Proyecto no encontrado",
                    a.CheckIn,
                    a.CheckOut,
                    a.TotalHours,
                    a.Notes,
                    a.RegisterType,
                    a.CreatedDate,
                    a.ModifiedDate,
                    a.CheckInDateOnly
                ));
                
                var response = APIResponse<IEnumerable<AssistanceResponseDto>>.SuccessResponse(
                    assistanceDtos, 
                    $"Se encontraron {totalCount} asistencias"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de asistencias");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener asistencia por ID
        /// </summary>
        /// <param name="id">ID de la asistencia</param>
        /// <returns>Asistencia encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<AssistanceResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAssistanceById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencia con ID: {Id}", id);
                
                var result = await _assistanceService.GetByIdWithIncludesAsync(id);
                
                if (result == null)
                {
                    return NotFound(APIResponse.ErrorResponse("Asistencia no encontrada", HttpStatusCode.NotFound));
                }
                
                var assistanceDto = new AssistanceResponseDto(
                    result.Id,
                    result.EmployeeId,
                    result.Employee?.FirstName + " " + result.Employee?.LastName ?? "Empleado no encontrado",
                    result.ProjectId,
                    result.Project?.ProjectName ?? "Proyecto no encontrado",
                    result.CheckIn,
                    result.CheckOut,
                    result.TotalHours,
                    result.Notes,
                    result.RegisterType,
                    result.CreatedDate,
                    result.ModifiedDate,
                    result.CheckInDateOnly
                );
                
                return Ok(APIResponse<AssistanceResponseDto>.SuccessResponse(assistanceDto, "Asistencia obtenida exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Registrar Check-In de un empleado
        /// </summary>
        /// <param name="dto">Datos del Check-In</param>
        /// <returns>Asistencia creada</returns>
        [HttpPost("check-in")]
        [ProducesResponseType(typeof(APIResponse<AssistanceResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            try
            {
                _logger.LogInformation("Registrando Check-In para empleado: {EmployeeId}", dto.EmployeeId);
                
                var result = await _assistanceService.CheckInAsync(dto);
                
                var assistanceDto = new AssistanceResponseDto(
                    result.Id,
                    result.EmployeeId,
                    "Empleado", // Se puede obtener después si es necesario
                    result.ProjectId,
                    "Proyecto", // Se puede obtener después si es necesario
                    result.CheckIn,
                    result.CheckOut,
                    result.TotalHours,
                    result.Notes,
                    result.RegisterType,
                    result.CreatedDate,
                    result.ModifiedDate,
                    result.CheckInDateOnly
                );
                
                var response = APIResponse<AssistanceResponseDto>.SuccessResponse(
                    assistanceDto, 
                    "Check-In registrado exitosamente"
                );
                
                return CreatedAtAction(nameof(GetAssistanceById), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar Check-In para empleado: {EmployeeId}", dto.EmployeeId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Registrar Check-Out de un empleado
        /// </summary>
        /// <param name="dto">Datos del Check-Out</param>
        /// <returns>Asistencia actualizada</returns>
        [HttpPost("check-out")]
        [ProducesResponseType(typeof(APIResponse<AssistanceResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutDto dto)
        {
            try
            {
                _logger.LogInformation("Registrando Check-Out para asistencia: {AssistanceId}", dto.AssistanceId);
                
                var result = await _assistanceService.CheckOutAsync(dto);
                
                var assistanceDto = new AssistanceResponseDto(
                    result.Id,
                    result.EmployeeId,
                    "Empleado", // Se puede obtener después si es necesario
                    result.ProjectId,
                    "Proyecto", // Se puede obtener después si es necesario
                    result.CheckIn,
                    result.CheckOut,
                    result.TotalHours,
                    result.Notes,
                    result.RegisterType,
                    result.CreatedDate,
                    result.ModifiedDate,
                    result.CheckInDateOnly
                );
                
                var response = APIResponse<AssistanceResponseDto>.SuccessResponse(
                    assistanceDto, 
                    "Check-Out registrado exitosamente"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar Check-Out para asistencia: {AssistanceId}", dto.AssistanceId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Actualizar asistencia existente
        /// </summary>
        /// <param name="id">ID de la asistencia</param>
        /// <param name="dto">Datos actualizados</param>
        /// <returns>Asistencia actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<AssistanceResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateAssistance(int id, [FromBody] UpdateAssistanceDto dto)
        {
            try
            {
                _logger.LogInformation("Actualizando asistencia con ID: {Id}", id);
                
                if (dto.Id != id)
                {
                    return BadRequest(APIResponse.ErrorResponse("El ID de la asistencia no coincide con la URL"));
                }
                
                var result = await _assistanceService.UpdateAsync(id, dto);
                
                var assistanceDto = new AssistanceResponseDto(
                    result.Id,
                    result.EmployeeId,
                    "Empleado", // Se puede obtener después si es necesario
                    result.ProjectId,
                    "Proyecto", // Se puede obtener después si es necesario
                    result.CheckIn,
                    result.CheckOut,
                    result.TotalHours,
                    result.Notes,
                    result.RegisterType,
                    result.CreatedDate,
                    result.ModifiedDate,
                    result.CheckInDateOnly
                );
                
                var response = APIResponse<AssistanceResponseDto>.SuccessResponse(
                    assistanceDto, 
                    "Asistencia actualizada exitosamente"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Eliminar asistencia (soft delete)
        /// </summary>
        /// <param name="id">ID de la asistencia</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAssistance(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando asistencia con ID: {Id}", id);
                
                await _assistanceService.DeleteAsync(id);
                
                var response = APIResponse.SuccessResponse("Asistencia eliminada exitosamente");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar asistencia con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener estadísticas de asistencias
        /// </summary>
        /// <param name="fromDate">Fecha de inicio</param>
        /// <param name="toDate">Fecha de fin</param>
        /// <returns>Estadísticas de asistencias</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAssistanceStatistics([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de asistencias desde {FromDate} hasta {ToDate}", fromDate, toDate);
                
                var statistics = await _assistanceService.GetStatisticsAsync(fromDate, toDate);
                
                var response = APIResponse<object>.SuccessResponse(statistics, "Estadísticas obtenidas exitosamente");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de asistencias");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
