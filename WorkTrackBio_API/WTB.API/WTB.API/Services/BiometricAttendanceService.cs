using WTB.API.Models.DTOs.Assistance;
using WTB.API.Models.Entities;
using WTB.API.Models.Enums;
using WTB.API.Services.Interfaces;

namespace WTB.API.Services
{
    /// <summary>
    /// Servicio para manejo automático de asistencia biométrica por proyecto
    /// </summary>
    public class BiometricAttendanceService : IBiometricAttendanceService
    {
        // TODO: Inyectar dependencias cuando esté listo el DbContext
        // private readonly WTBDbContext _context;
        // private readonly ILogger<BiometricAttendanceService> _logger;

        /// <summary>
        /// Procesa automáticamente la asistencia basada en lectura biométrica
        /// </summary>
        /// <param name="deviceId">ID único del dispositivo lector</param>
        /// <param name="fingerprintTemplate">Template de la huella escaneada</param>
        /// <returns>Resultado del procesamiento</returns>
        public async Task<BiometricAttendanceResult> ProcessBiometricAttendance(
            string deviceId, 
            byte[] fingerprintTemplate)
        {
            try
            {
                // 1. Verificar que el dispositivo existe y está activo
                var device = await GetDeviceByIdAsync(deviceId);
                if (device == null)
                {
                    return new BiometricAttendanceResult
                    {
                        Success = false,
                        Message = "❌ Dispositivo no registrado",
                        ErrorCode = "DEVICE_NOT_FOUND"
                    };
                }

                if (!device.IsActive)
                {
                    return new BiometricAttendanceResult
                    {
                        Success = false,
                        Message = "❌ Dispositivo inactivo",
                        ErrorCode = "DEVICE_INACTIVE"
                    };
                }

                // 2. Identificar empleado por huella
                var employee = await IdentifyEmployeeByFingerprintAsync(fingerprintTemplate);
                if (employee == null)
                {
                    return new BiometricAttendanceResult
                    {
                        Success = false,
                        Message = "❌ Huella no reconocida",
                        ErrorCode = "FINGERPRINT_NOT_FOUND"
                    };
                }

                // 3. Verificar que el empleado esté asignado al proyecto
                var isAssigned = await IsEmployeeAssignedToProjectAsync(employee.Id, device.ProjectId);
                if (!isAssigned)
                {
                    return new BiometricAttendanceResult
                    {
                        Success = false,
                        Message = $"❌ {employee.FirstName} no está asignado al proyecto {device.Project.ProjectName}",
                        ErrorCode = "EMPLOYEE_NOT_ASSIGNED"
                    };
                }

                // 4. Obtener último registro de asistencia del empleado
                var lastAssistance = await GetLastActiveAssistanceAsync(employee.Id);

                // 5. Determinar si debe ser CheckIn o CheckOut
                bool shouldBeCheckOut = lastAssistance != null && 
                                      lastAssistance.CheckOut == null &&
                                      lastAssistance.CheckInDateOnly.Date == DateTime.Today;

                // 6. Procesar la acción correspondiente
                if (shouldBeCheckOut && lastAssistance != null)
                {
                    // Hacer CheckOut
                    var checkOutResult = await ProcessCheckOutAsync(lastAssistance, device);
                    return new BiometricAttendanceResult
                    {
                        Success = true,
                        Message = $"✅ CheckOut registrado - ¡Hasta luego {employee.FirstName}!",
                        ActionType = "CheckOut",
                        EmployeeName = $"{employee.FirstName} {employee.LastName}",
                        ProjectName = device.Project.ProjectName,
                        TotalHours = checkOutResult.TotalHours,
                        AssistanceId = checkOutResult.Id
                    };
                }
                else
                {
                    // Hacer CheckIn
                    var checkInResult = await ProcessCheckInAsync(employee.Id, device);
                    return new BiometricAttendanceResult
                    {
                        Success = true,
                        Message = $"✅ CheckIn registrado - ¡Bienvenido {employee.FirstName}!",
                        ActionType = "CheckIn",
                        EmployeeName = $"{employee.FirstName} {employee.LastName}",
                        ProjectName = device.Project.ProjectName,
                        TotalHours = 0,
                        AssistanceId = checkInResult.Id
                    };
                }
            }
            catch (Exception)
            {
                // TODO: Log error cuando esté configurado
                return new BiometricAttendanceResult
                {
                    Success = false,
                    Message = "❌ Error interno del sistema",
                    ErrorCode = "INTERNAL_ERROR"
                };
            }
        }

        /// <summary>
        /// Obtiene dispositivo por ID
        /// </summary>
        private async Task<Device?> GetDeviceByIdAsync(string deviceId)
        {
            // TODO: Implementar consulta a base de datos
            // return await _context.Devices
            //     .Include(d => d.Project)
            //     .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
            
            // Mock temporal
            await Task.Delay(1);
            return new Device 
            { 
                Id = 1, 
                DeviceId = deviceId, 
                ProjectId = 1, 
                IsActive = true,
                Project = new Projects { Id = 1, ProjectName = "Mall San Pedro CCTV" }
            };
        }

        /// <summary>
        /// Identifica empleado por template de huella
        /// </summary>
        private async Task<EmployeeInfo?> IdentifyEmployeeByFingerprintAsync(byte[] template)
        {
            // TODO: Implementar algoritmo de coincidencia biométrica
            // 1. Obtener todas las huellas de la base de datos
            // 2. Comparar template con cada huella registrada
            // 3. Retornar empleado si coincidencia > threshold (ej: 85%)
            
            await Task.Delay(1);
            return new EmployeeInfo 
            { 
                Id = 1, 
                FirstName = "Roberto", 
                LastName = "García" 
            };
        }

        /// <summary>
        /// Verifica si empleado está asignado al proyecto
        /// </summary>
        private async Task<bool> IsEmployeeAssignedToProjectAsync(int employeeId, int projectId)
        {
            // TODO: Consultar tabla ProjectsAssigns
            await Task.Delay(1);
            return true; // Mock temporal
        }

        /// <summary>
        /// Obtiene último registro activo de asistencia
        /// </summary>
        private async Task<Assistance?> GetLastActiveAssistanceAsync(int employeeId)
        {
            // TODO: Implementar consulta
            await Task.Delay(1);
            return null; // Mock - simula que no hay CheckIn activo
        }

        /// <summary>
        /// Procesa CheckIn automático
        /// </summary>
        private Task<Assistance> ProcessCheckInAsync(int employeeId, Device device)
        {
            var assistance = new Assistance
            {
                EmployeeId = employeeId,
                ProjectId = device.ProjectId,
                CheckIn = DateTime.Now,
                RegisterType = RegisterTypeEnum.CheckIn.ToString(),
                Notes = $"CheckIn automático desde {device.DeviceName}"
            };

            // TODO: Guardar en base de datos
            assistance.Id = 1; // Mock ID
            return Task.FromResult(assistance);
        }

        /// <summary>
        /// Procesa CheckOut automático
        /// </summary>
        private Task<Assistance> ProcessCheckOutAsync(Assistance assistance, Device device)
        {
            assistance.CheckOut = DateTime.Now;
            assistance.ModifiedDate = DateTime.Now;
            assistance.RegisterType = RegisterTypeEnum.CheckOut.ToString();
            
            // Calcular horas trabajadas
            var workedTime = assistance.CheckOut.Value - assistance.CheckIn;
            assistance.TotalHours = (decimal)workedTime.TotalHours;
            
            assistance.Notes += $" | CheckOut automático desde {device.DeviceName}";

            // TODO: Actualizar en base de datos
            return Task.FromResult(assistance);
        }
    }

    /// <summary>
    /// Resultado del procesamiento biométrico
    /// </summary>
    public class BiometricAttendanceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ActionType { get; set; } // "CheckIn" o "CheckOut"
        public string? EmployeeName { get; set; }
        public string? ProjectName { get; set; }
        public decimal? TotalHours { get; set; }
        public int? AssistanceId { get; set; }
        public string? ErrorCode { get; set; }
    }
}



