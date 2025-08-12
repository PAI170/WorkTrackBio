using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Device;
using WTB.API.Services;
using WTB.API.Validators.Device;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para control de acceso automático a dispositivos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<ActionResult<DeviceAccessResponseDto>> ProcessAccess([FromBody] DeviceAccessRequestDto request)
        {
            try
            {
                // Validar la solicitud
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                    return BadRequest(new
                    {
                        Message = "Datos de entrada inválidos",
                        Errors = errors
                    });
                }

                // Procesar el acceso
                var result = await _deviceAccessService.ProcessDeviceAccessAsync(request);

                if (result.IsAccessGranted)
                {
                    _logger.LogInformation("Acceso exitoso: Empleado {EmployeeId} en dispositivo {DeviceId} - {AccessType}", 
                        request.EmployeeId, request.DeviceId, request.AccessType);
                    
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Acceso denegado: Empleado {EmployeeId} en dispositivo {DeviceId} - {Message}", 
                        request.EmployeeId, request.DeviceId, result.Message);
                    
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando acceso al dispositivo {DeviceId} para empleado {EmployeeId}", 
                    request.DeviceId, request.EmployeeId);
                
                return StatusCode(500, new
                {
                    Message = "Error interno del sistema",
                    Error = "Contacte al administrador"
                });
            }
        }

        /// <summary>
        /// Verifica si un usuario tiene acceso a un proyecto específico
        /// </summary>
        /// <param name="employeeId">ID del empleado</param>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Estado del acceso</returns>
        [HttpGet("validate-access/{employeeId}/{projectId}")]
        public async Task<ActionResult<object>> ValidateUserAccess(int employeeId, int projectId)
        {
            try
            {
                var hasAccess = await _deviceAccessService.ValidateUserProjectAccessAsync(employeeId, projectId);
                
                return Ok(new
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    HasAccess = hasAccess,
                    Message = hasAccess ? "Acceso permitido" : "Acceso denegado"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando acceso del empleado {EmployeeId} al proyecto {ProjectId}", 
                    employeeId, projectId);
                
                return StatusCode(500, new
                {
                    Message = "Error interno del sistema",
                    Error = "Contacte al administrador"
                });
            }
        }

        /// <summary>
        /// Endpoint de salud para verificar que el controlador esté funcionando
        /// </summary>
        /// <returns>Estado del servicio</returns>
        [HttpGet("health")]
        public ActionResult<object> Health()
        {
            return Ok(new
            {
                Status = "Healthy",
                Service = "Device Access Control",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            });
        }
    }
}
