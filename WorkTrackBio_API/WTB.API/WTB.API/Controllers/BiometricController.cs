using Microsoft.AspNetCore.Mvc;
using WTB.API.Helpers;
using WTB.API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para manejo de dispositivos biométricos y asistencia automática
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BiometricController : ControllerBase
    {
        private readonly IBiometricAttendanceService _biometricService;
        private readonly ILogger<BiometricController> _logger;

        public BiometricController(
            IBiometricAttendanceService biometricService,
            ILogger<BiometricController> logger)
        {
            _biometricService = biometricService;
            _logger = logger;
        }

        /// <summary>
        /// Procesa asistencia automática desde dispositivo biométrico
        /// </summary>
        /// <param name="request">Datos del escaneo biométrico</param>
        /// <returns>Resultado del procesamiento de asistencia</returns>
        [HttpPost("attendance")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ProcessBiometricAttendance([FromBody] BiometricAttendanceRequest request)
        {
            try
            {
                _logger.LogInformation("Procesando asistencia biométrica para dispositivo: {DeviceId}", request.DeviceId);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                // Convertir template de Base64 a bytes
                byte[] fingerprintTemplate;
                try
                {
                    fingerprintTemplate = Convert.FromBase64String(request.FingerprintTemplate);
                }
                catch
                {
                    return BadRequest(APIResponse.ErrorResponse("Template de huella inválido"));
                }

                // Procesar asistencia biométrica
                var result = await _biometricService.ProcessBiometricAttendance(
                    request.DeviceId, 
                    fingerprintTemplate);

                if (!result.Success)
                {
                    var errorMessage = result.ErrorCode switch
                    {
                        "DEVICE_NOT_FOUND" => "Dispositivo no encontrado",
                        "DEVICE_INACTIVE" => "Dispositivo inactivo",
                        "FINGERPRINT_NOT_FOUND" => "Huella no reconocida",
                        "EMPLOYEE_NOT_ASSIGNED" => result.Message,
                        _ => "Error procesando asistencia"
                    };

                    var statusCode = result.ErrorCode switch
                    {
                        "DEVICE_NOT_FOUND" => HttpStatusCode.NotFound,
                        "DEVICE_INACTIVE" => HttpStatusCode.BadRequest,
                        "FINGERPRINT_NOT_FOUND" => HttpStatusCode.NotFound,
                        "EMPLOYEE_NOT_ASSIGNED" => HttpStatusCode.BadRequest,
                        _ => HttpStatusCode.InternalServerError
                    };

                    return StatusCode((int)statusCode, APIResponse.ErrorResponse(errorMessage, statusCode));
                }

                var response = new
                {
                    message = result.Message,
                    actionType = result.ActionType,
                    employeeName = result.EmployeeName,
                    projectName = result.ProjectName,
                    totalHours = result.TotalHours,
                    assistanceId = result.AssistanceId,
                    timestamp = DateTime.Now
                };

                _logger.LogInformation("Asistencia biométrica procesada exitosamente para empleado: {EmployeeName}", result.EmployeeName);

                return Ok(APIResponse<object>.SuccessResponse(response, "Asistencia biométrica procesada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando asistencia biométrica para dispositivo: {DeviceId}", request.DeviceId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene status de un dispositivo específico
        /// </summary>
        /// <param name="deviceId">ID del dispositivo</param>
        /// <returns>Estado del dispositivo</returns>
        [HttpGet("device/{deviceId}/status")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDeviceStatus([FromRoute] string deviceId)
        {
            try
            {
                _logger.LogInformation("Obteniendo status del dispositivo: {DeviceId}", deviceId);

                // TODO: Implementar consulta real al servicio
                // Por ahora retornamos datos simulados hasta que se implemente el servicio completo
                await Task.Delay(1);

                var deviceStatus = new
                {
                    deviceId = deviceId,
                    isActive = true,
                    lastSeen = DateTime.Now.AddMinutes(-5),
                    projectName = "Mall San Pedro CCTV",
                    location = "Entrada Principal",
                    totalScansToday = 24,
                    lastEmployee = "Roberto García",
                    lastAction = "CheckOut",
                    lastActionTime = DateTime.Now.AddMinutes(-15)
                };

                return Ok(APIResponse<object>.SuccessResponse(deviceStatus, "Status del dispositivo obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo status del dispositivo: {DeviceId}", deviceId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Simula un escaneo biométrico para pruebas
        /// </summary>
        /// <param name="request">Datos de simulación</param>
        /// <returns>Resultado simulado</returns>
        [HttpPost("simulate")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SimulateBiometricScan([FromBody] BiometricSimulationRequest request)
        {
            try
            {
                _logger.LogInformation("Simulando escaneo biométrico para empleado: {EmployeeId} en dispositivo: {DeviceId}", 
                    request.EmployeeId, request.DeviceId);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de simulación inválidos: {string.Join(", ", errors)}"));
                }

                // Generar template falso para simulación
                var fakeTemplate = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"FAKE_TEMPLATE_{request.EmployeeId}_{DateTime.Now.Ticks}"));

                var biometricRequest = new BiometricAttendanceRequest
                {
                    DeviceId = request.DeviceId,
                    FingerprintTemplate = fakeTemplate
                };

                // Procesar como si fuera un escaneo real
                var result = await _biometricService.ProcessBiometricAttendance(
                    biometricRequest.DeviceId,
                    Convert.FromBase64String(biometricRequest.FingerprintTemplate));

                var response = new
                {
                    message = $"🧪 SIMULACIÓN - {result.Message}",
                    isSimulation = true,
                    actionType = result.ActionType,
                    employeeName = result.EmployeeName,
                    projectName = result.ProjectName,
                    timestamp = DateTime.Now
                };

                _logger.LogInformation("Simulación biométrica completada exitosamente para empleado: {EmployeeName}", result.EmployeeName);

                return Ok(APIResponse<object>.SuccessResponse(response, "Simulación biométrica completada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en simulación biométrica para empleado: {EmployeeId} en dispositivo: {DeviceId}", 
                    request.EmployeeId, request.DeviceId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }

    /// <summary>
    /// Solicitud para procesamiento de asistencia biométrica
    /// </summary>
    public class BiometricAttendanceRequest
    {
        [Required(ErrorMessage = "El ID del dispositivo es obligatorio")]
        [StringLength(50, ErrorMessage = "El ID del dispositivo no puede exceder 50 caracteres")]
        public string DeviceId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El template de huella es obligatorio")]
        public string FingerprintTemplate { get; set; } = string.Empty;

        public string? Location { get; set; }
        
        public DateTime? Timestamp { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Solicitud para simulación de escaneo biométrico
    /// </summary>
    public class BiometricSimulationRequest
    {
        [Required(ErrorMessage = "El ID del dispositivo es obligatorio")]
        public string DeviceId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID del empleado es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "ID de empleado inválido")]
        public int EmployeeId { get; set; }
    }
}
