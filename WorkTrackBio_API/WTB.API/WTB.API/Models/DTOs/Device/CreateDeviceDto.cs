using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO para registrar un nuevo dispositivo lector
    /// </summary>
    public record CreateDeviceDto(
        [Required]
        [StringLength(50, ErrorMessage = "El ID del dispositivo no puede exceder 50 caracteres")]
        string DeviceId,

        [Required]
        [StringLength(100, ErrorMessage = "El nombre del dispositivo no puede exceder 100 caracteres")]
        string DeviceName,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proyecto válido")]
        int ProjectId,

        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        string? Location = null,

        [StringLength(50, ErrorMessage = "El tipo de dispositivo no puede exceder 50 caracteres")]
        string DeviceType = "Fingerprint",

        [StringLength(100, ErrorMessage = "La dirección IP no puede exceder 100 caracteres")]
        string? IpAddress = null,

        [StringLength(20, ErrorMessage = "El número de serie no puede exceder 20 caracteres")]
        string? SerialNumber = null,

        [StringLength(50, ErrorMessage = "La versión de firmware no puede exceder 50 caracteres")]
        string? FirmwareVersion = null,

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        string? Notes = null
    );
}
