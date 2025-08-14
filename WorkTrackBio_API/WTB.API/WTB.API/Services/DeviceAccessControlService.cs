using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using WTB.API.Data.Context;
using WTB.API.Models.DTOs.Device;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;

namespace WTB.API.Services
{
    /// <summary>
    /// Servicio para control de acceso automático a dispositivos
    /// </summary>
    public class DeviceAccessControlService : IDeviceAccessControlService
    {
        private readonly WTBDbContext _context;
        private readonly ILogger<DeviceAccessControlService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeviceAccessControlService(
            WTBDbContext context, 
            ILogger<DeviceAccessControlService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Procesa el acceso del usuario al dispositivo
        /// </summary>
        public async Task<DeviceAccessResponseDto> ProcessDeviceAccessAsync(DeviceAccessRequestDto request)
        {
            try
            {
                // 1. Obtener información del dispositivo
                var device = await GetDeviceWithProjectAsync(request.DeviceId);
                if (device == null)
                {
                    return new DeviceAccessResponseDto(
                        IsAccessGranted: false,
                        Message: "Dispositivo no encontrado o inactivo"
                    );
                }

                // 2. Obtener información del empleado
                var employee = await GetEmployeeAsync(request.EmployeeId);
                if (employee == null)
                {
                    return new DeviceAccessResponseDto(
                        IsAccessGranted: false,
                        Message: "Empleado no encontrado"
                    );
                }

                // 3. Validar acceso al proyecto
                var hasAccess = await ValidateUserProjectAccessAsync(request.EmployeeId, device.ProjectId);
                if (!hasAccess)
                {
                    return new DeviceAccessResponseDto(
                        IsAccessGranted: false,
                        Message: $"El empleado {employee.FirstName} {employee.LastName} no tiene acceso activo al proyecto {device.Project.ProjectName}",
                        ProjectName: device.Project.ProjectName,
                        EmployeeName: $"{employee.FirstName} {employee.LastName}",
                        DeviceName: device.DeviceName
                    );
                }

                // 4. Procesar el tipo de acceso
                var assistanceId = await ProcessAccessTypeAsync(request, device, employee);
                
                // 5. Actualizar último acceso del dispositivo
                await UpdateDeviceLastSeenAsync(device.Id);

                var accessTime = DateTime.UtcNow;
                var message = request.AccessType.ToUpper() == "CHECKIN" 
                    ? $"Check-in exitoso para {employee.FirstName} {employee.LastName} en {device.Project.ProjectName}"
                    : $"Check-out exitoso para {employee.FirstName} {employee.LastName} en {device.Project.ProjectName}";

                return new DeviceAccessResponseDto(
                    IsAccessGranted: true,
                    Message: message,
                    AssistanceId: assistanceId,
                    AccessTime: accessTime,
                    ProjectName: device.Project.ProjectName,
                    EmployeeName: $"{employee.FirstName} {employee.LastName}",
                    DeviceName: device.DeviceName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando acceso al dispositivo {DeviceId} para empleado {EmployeeId}", 
                    request.DeviceId, request.EmployeeId);
                
                return new DeviceAccessResponseDto(
                    IsAccessGranted: false,
                    Message: "Error interno del sistema. Contacte al administrador."
                );
            }
        }

        /// <summary>
        /// Valida si un usuario tiene acceso activo a un proyecto
        /// </summary>
        public async Task<bool> ValidateUserProjectAccessAsync(int employeeId, int projectId)
        {
            var assignment = await _context.ProjectsAssigns
                .Where(pa => pa.EmployeeId == employeeId && pa.ProjectId == projectId)
                .OrderByDescending(pa => pa.AssignDate)
                .FirstOrDefaultAsync();

            if (assignment == null)
                return false;

            // Verificar que la asignación esté activa (sin fecha de fin o fecha futura)
            return assignment.EndDate == null || assignment.EndDate > DateTime.UtcNow;
        }

        /// <summary>
        /// Obtiene el dispositivo con información del proyecto
        /// </summary>
        private async Task<Device?> GetDeviceWithProjectAsync(int deviceId)
        {
            return await _context.Devices
                .Include(d => d.Project)
                .Where(d => d.Id == deviceId && d.IsActive)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtiene información del empleado
        /// </summary>
        private async Task<EmployeeInfo?> GetEmployeeAsync(int employeeId)
        {
            return await _context.EmployeeInfo
                .Where(e => e.Id == employeeId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Procesa el tipo de acceso (CHECKIN o CHECKOUT)
        /// </summary>
        private async Task<int?> ProcessAccessTypeAsync(DeviceAccessRequestDto request, Device device, EmployeeInfo employee)
        {
            if (request.AccessType.ToUpper() == "CHECKIN")
            {
                return await ProcessCheckInAsync(request, device, employee);
            }
            else
            {
                return await ProcessCheckOutAsync(request, device, employee);
            }
        }

        /// <summary>
        /// Procesa el check-in del empleado
        /// </summary>
        private async Task<int> ProcessCheckInAsync(DeviceAccessRequestDto request, Device device, EmployeeInfo employee)
        {
            // Verificar si ya existe un check-in sin check-out para hoy
            var existingAssistance = await _context.Assistance
                .Where(a => a.EmployeeId == request.EmployeeId && 
                           a.ProjectId == device.ProjectId &&
                           a.CheckInDateOnly == DateTime.UtcNow.Date &&
                           a.CheckOut == null)
                .FirstOrDefaultAsync();

            if (existingAssistance != null)
            {
                // Ya existe un check-in sin check-out, actualizar notas
                existingAssistance.Notes = $"Check-in múltiple registrado desde {device.DeviceName}. {request.Notes}";
                existingAssistance.MarkAsModified(GetCurrentUserId());
                await _context.SaveChangesAsync();
                return existingAssistance.Id;
            }

            // Crear nueva asistencia
            var assistance = new Assistance
            {
                EmployeeId = request.EmployeeId,
                ProjectId = device.ProjectId,
                DeviceId = device.Id,
                CheckIn = DateTime.UtcNow,
                RegisterType = "DEVICE",
                Notes = request.Notes
            };

            _context.Assistance.Add(assistance);
            await _context.SaveChangesAsync();
            
            return assistance.Id;
        }

        /// <summary>
        /// Procesa el check-out del empleado
        /// </summary>
        private async Task<int?> ProcessCheckOutAsync(DeviceAccessRequestDto request, Device device, EmployeeInfo employee)
        {
            // Buscar asistencia activa para hoy
            var activeAssistance = await _context.Assistance
                .Where(a => a.EmployeeId == request.EmployeeId && 
                           a.ProjectId == device.ProjectId &&
                           a.CheckInDateOnly == DateTime.UtcNow.Date &&
                           a.CheckOut == null)
                .FirstOrDefaultAsync();

            if (activeAssistance == null)
            {
                // No hay check-in activo, crear uno automáticamente
                var assistance = new Assistance
                {
                    EmployeeId = request.EmployeeId,
                    ProjectId = device.ProjectId,
                    DeviceId = device.Id,
                    CheckIn = DateTime.UtcNow.AddHours(-8), // Asumir 8 horas de trabajo
                    CheckOut = DateTime.UtcNow,
                    RegisterType = "DEVICE_AUTO",
                    Notes = $"Check-out automático sin check-in previo. {request.Notes}"
                };

                _context.Assistance.Add(assistance);
                await _context.SaveChangesAsync();
                return assistance.Id;
            }

            // Actualizar check-out existente
            activeAssistance.CheckOut = DateTime.UtcNow;
            activeAssistance.Notes = $"{activeAssistance.Notes}. Check-out desde {device.DeviceName}. {request.Notes}".Trim();
            activeAssistance.MarkAsModified(GetCurrentUserId());
            
            await _context.SaveChangesAsync();
            return activeAssistance.Id;
        }

        /// <summary>
        /// Obtiene el ID del usuario actual del contexto HTTP
        /// </summary>
        private int GetCurrentUserId()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = httpContext.User.FindFirst("UserId") ?? 
                                    httpContext.User.FindFirst("sub") ?? 
                                    httpContext.User.FindFirst("nameidentifier");
                    
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                }
            }
            catch (Exception)
            {
                // Si hay error, continuar con usuario del sistema
            }

            // Usuario del sistema por defecto
            return 1;
        }

        /// <summary>
        /// Actualiza el último acceso del dispositivo
        /// </summary>
        private async Task UpdateDeviceLastSeenAsync(int deviceId)
        {
            var device = await _context.Devices.FindAsync(deviceId);
            if (device != null)
            {
                device.LastSeen = DateTime.UtcNow;
                device.MarkAsModified(GetCurrentUserId());
                await _context.SaveChangesAsync();
            }
        }
    }
}
