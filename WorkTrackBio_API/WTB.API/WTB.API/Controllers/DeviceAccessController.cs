using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Device;
using WTB.API.Services.Interfaces;
using WTB.API.Validators.Device;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para control de acceso automático a dispositivos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DeviceAccessController : ControllerBase
    {
        private readonly IDeviceAccessControlService _deviceAccessService;
        private readonly DeviceAccessRequestValidator _validator;
        private readonly ILogger<DeviceAccessController> _logger;

        public DeviceAccessController(
            IDeviceAccessControlService deviceAccessService,
            DeviceAccessRequestValidator validator,
            ILogger<DeviceAccessController> logger)
        {
            _deviceAccessService = deviceAccessService;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Procesa el acceso de un usuario a un dispositivo
        /// </summary>
        /// <param name="request">Solicitud de acceso</param>
        /// <returns>Resultado del acceso</returns>
        [HttpPost("access")]
        [ProducesResponseType(typeof(APIResponse<DeviceAccessResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ProcessAccess([FromBody] DeviceAccessRequestDto request)
        {
            try
            {
                _logger.LogInformation("Procesando acceso al dispositivo {DeviceId} para empleado {EmployeeId}", 
                    request.DeviceId, request.EmployeeId);

                // Validar la solicitud
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                    var errorMessage = $"Datos de entrada inválidos: {string.Join(", ", errors)}";
                    
                    _logger.LogWarning("Validación fallida para acceso al dispositivo {DeviceId}: {Errors}", 
                        request.DeviceId, string.Join(", ", errors));
                    
                    return BadRequest(APIResponse.ErrorResponse(errorMessage));
                }

                // Procesar el acceso
                var result = await _deviceAccessService.ProcessDeviceAccessAsync(request);

                if (result.IsAccessGranted)
                {
                    _logger.LogInformation("Acceso exitoso: Empleado {EmployeeId} en dispositivo {DeviceId} - {AccessType}", 
                        request.EmployeeId, request.DeviceId, request.AccessType);
                    
                    return Ok(APIResponse<DeviceAccessResponseDto>.SuccessResponse(result, "Acceso al dispositivo concedido exitosamente"));
                }
                else
                {
                    _logger.LogWarning("Acceso denegado: Empleado {EmployeeId} en dispositivo {DeviceId} - {Message}", 
                        request.EmployeeId, request.DeviceId, result.Message);
                    
                    return BadRequest(APIResponse.ErrorResponse(result.Message));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando acceso al dispositivo {DeviceId} para empleado {EmployeeId}", 
                    request.DeviceId, request.EmployeeId);
                
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del sistema", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Verifica si un usuario tiene acceso a un proyecto específico
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Estado del acceso</returns>
        [HttpGet("validate-access/{employeeId}/{projectId}")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ValidateUserAccess(int employeeId, int projectId)
        {
            try
            {
                _logger.LogInformation("Validando acceso del empleado {EmployeeId} al proyecto {ProjectId}", 
                    employeeId, projectId);

                var hasAccess = await _deviceAccessService.ValidateUserProjectAccessAsync(employeeId, projectId);
                
                var response = new
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    HasAccess = hasAccess,
                    Message = hasAccess ? "Acceso permitido" : "Acceso denegado"
                };

                _logger.LogInformation("Validación de acceso completada para empleado {EmployeeId} al proyecto {ProjectId}: {Result}", 
                    employeeId, projectId, hasAccess ? "PERMITIDO" : "DENEGADO");

                return Ok(APIResponse<object>.SuccessResponse(response, 
                    hasAccess ? "Acceso validado exitosamente" : "Acceso denegado"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando acceso del empleado {EmployeeId} al proyecto {ProjectId}", 
                    employeeId, projectId);
                
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del sistema", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Endpoint de salud para verificar que el controlador esté funcionando
        /// </summary>
        /// <returns>Estado del servicio</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        public IActionResult Health()
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Service = "Device Access Control",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            };

            return Ok(APIResponse<object>.SuccessResponse(healthStatus, "Servicio funcionando correctamente"));
        }
    }
}
