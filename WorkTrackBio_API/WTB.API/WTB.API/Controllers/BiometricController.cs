using Microsoft.AspNetCore.Mvc;
using WTB.API.Helpers;
using WTB.API.Services;
using System.ComponentModel.DataAnnotations;

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
        private readonly BiometricAttendanceService _biometricService;

        public BiometricController()
        {
            // TODO: Inyectar dependencia cuando esté configurado DI
            _biometricService = new BiometricAttendanceService();
        }

        /// <summary>
        /// Procesa asistencia automática desde dispositivo biométrico
        /// </summary>
        /// <param name="request">Datos del escaneo biométrico</param>
        /// <returns>Resultado del procesamiento de asistencia</returns>
        [HttpPost("attendance")]
        public async Task<IActionResult> ProcessBiometricAttendance([FromBody] BiometricAttendanceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return HttpErrors.ValidationError(ModelState);
                }

                // Convertir template de Base64 a bytes
                byte[] fingerprintTemplate;
                try
                {
                    fingerprintTemplate = Convert.FromBase64String(request.FingerprintTemplate);
                }
                catch
                {
                    return HttpErrors.BadRequest("Template de huella inválido");
                }

                // Procesar asistencia biométrica
                var result = await _biometricService.ProcessBiometricAttendance(
                    request.DeviceId, 
                    fingerprintTemplate);

                if (!result.Success)
                {
                    return result.ErrorCode switch
                    {
                        "DEVICE_NOT_FOUND" => HttpErrors.NotFound("Dispositivo no encontrado"),
                        "DEVICE_INACTIVE" => HttpErrors.BadRequest("Dispositivo inactivo"),
                        "FINGERPRINT_NOT_FOUND" => HttpErrors.NotFound("Huella no reconocida"),
                        "EMPLOYEE_NOT_ASSIGNED" => HttpErrors.BadRequest(result.Message),
                        _ => HttpErrors.InternalServerError("Error procesando asistencia")
                    };
                }

                return Ok(APIResponse<object>.SuccessResponse(new
                {
                    message = result.Message,
                    actionType = result.ActionType,
                    employeeName = result.EmployeeName,
                    projectName = result.ProjectName,
                    totalHours = result.TotalHours,
                    assistanceId = result.AssistanceId,
                    timestamp = DateTime.Now
                }));
            }
            catch (Exception ex)
            {
                // TODO: Log error
                return HttpErrors.InternalServerError("Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene status de un dispositivo específico
        /// </summary>
        /// <param name="deviceId">ID del dispositivo</param>
        /// <returns>Estado del dispositivo</returns>
        [HttpGet("device/{deviceId}/status")]
        public async Task<IActionResult> GetDeviceStatus([FromRoute] string deviceId)
        {
            try
            {
                // TODO: Implementar consulta real
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

                return Ok(APIResponse<object>.SuccessResponse(deviceStatus));
            }
            catch (Exception ex)
            {
                return HttpErrors.InternalServerError("Error obteniendo status del dispositivo");
            }
        }

        /// <summary>
        /// Simula un escaneo biométrico para pruebas
        /// </summary>
        /// <param name="request">Datos de simulación</param>
        /// <returns>Resultado simulado</returns>
        [HttpPost("simulate")]
        public async Task<IActionResult> SimulateBiometricScan([FromBody] BiometricSimulationRequest request)
        {
            try
            {
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

                return Ok(APIResponse<object>.SuccessResponse(new
                {
                    message = $"🧪 SIMULACIÓN - {result.Message}",
                    isSimulation = true,
                    actionType = result.ActionType,
                    employeeName = result.EmployeeName,
                    projectName = result.ProjectName,
                    timestamp = DateTime.Now
                }));
            }
            catch (Exception ex)
            {
                return HttpErrors.InternalServerError("Error en simulación");
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
        [Required]
        public string DeviceId { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ID de empleado inválido")]
        public int EmployeeId { get; set; }
    }
}
