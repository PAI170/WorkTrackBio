using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO para solicitar acceso a un dispositivo
    /// </summary>
    public record DeviceAccessRequestDto(
        [Required(ErrorMessage = "El ID del dispositivo es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del dispositivo debe ser un número válido")]
        int DeviceId,

        [Required(ErrorMessage = "El ID del empleado es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser un número válido")]
        int EmployeeId,

        [Required(ErrorMessage = "El tipo de acceso es requerido")]
        [RegularExpression("^(CHECKIN|CHECKOUT)$", ErrorMessage = "El tipo de acceso debe ser CHECKIN o CHECKOUT")]
        string AccessType,

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        string? Notes = null
    );
}
