using WTB.API.Models.DTOs.Assistance;

namespace WTB.API.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de asistencia biométrica
    /// </summary>
    public interface IBiometricAttendanceService
    {
        /// <summary>
        /// Procesa automáticamente la asistencia basada en lectura biométrica
        /// </summary>
        /// <param name="deviceId">ID único del dispositivo lector</param>
        /// <param name="fingerprintTemplate">Template de la huella escaneada</param>
        /// <returns>Resultado del procesamiento</returns>
        Task<BiometricAttendanceResult> ProcessBiometricAttendance(string deviceId, byte[] fingerprintTemplate);
    }
}
