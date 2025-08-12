using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO para actualizar información de un dispositivo
    /// </summary>
    public record UpdateDeviceDto(
        [Required(ErrorMessage = "El ID del dispositivo es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del dispositivo debe ser un número válido")]
        int Id,

        [StringLength(100, ErrorMessage = "El nombre del dispositivo no puede exceder 100 caracteres")]
        string? DeviceName = null,

        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        string? Location = null,

        [StringLength(50, ErrorMessage = "El tipo de dispositivo no puede exceder 50 caracteres")]
        string? DeviceType = null,

        bool? IsActive = null,

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
