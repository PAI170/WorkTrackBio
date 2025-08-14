using WTB.API.Models.DTOs.Device;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de control de acceso automático a dispositivos
    /// </summary>
    public interface IDeviceAccessControlService
    {
        /// <summary>
        /// Valida el acceso de un usuario a un dispositivo y registra la asistencia
        /// </summary>
        Task<DeviceAccessResponseDto> ProcessDeviceAccessAsync(DeviceAccessRequestDto request);
        
        /// <summary>
        /// Verifica si un usuario tiene acceso activo a un proyecto
        /// </summary>
        Task<bool> ValidateUserProjectAccessAsync(int employeeId, int projectId);
    }
}
