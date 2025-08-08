using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para registrar check-in de un empleado
    /// </summary>
    public record CheckInDto(
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado válido")]
        int EmployeeId,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proyecto válido")]
        int ProjectId,

        [Required]
        DateTime CheckIn,

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        string? Notes = null,

        // Información del dispositivo (opcional para registros manuales)
        [StringLength(50, ErrorMessage = "El ID del dispositivo no puede exceder 50 caracteres")]
        string? DeviceId = null
    );
}
