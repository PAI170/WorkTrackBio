using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para actualizar un registro de asistencia (solo para administradores)
    /// </summary>
    public record UpdateAssistanceDto(
        [Required]
        int Id,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado válido")]
        int EmployeeId,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proyecto válido")]
        int ProjectId,

        [Required]
        DateTime CheckIn,

        DateTime? CheckOut,

        [Range(0, 24, ErrorMessage = "Las horas totales deben estar entre 0 y 24")]
        decimal? TotalHours,

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        string? Notes,

        [Required]
        [StringLength(50, ErrorMessage = "El tipo de registro no puede exceder 50 caracteres")]
        string RegisterType
    );
}
