using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.AuditRegister
{
    /// <summary>
    /// DTO para crear un nuevo registro de auditoría
    /// </summary>
    public record CreateAuditRegisterDto(
        [Required(ErrorMessage = "El ID de asistencia es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID de asistencia debe ser un número válido")]
        int AssistanceId,

        [Required(ErrorMessage = "El tipo de acción es requerido")]
        [StringLength(50, ErrorMessage = "El tipo de acción no puede exceder 50 caracteres")]
        string ActionType,

        [Required(ErrorMessage = "El detalle del cambio es requerido")]
        [StringLength(1000, ErrorMessage = "El detalle del cambio no puede exceder 1000 caracteres")]
        string DetailChange,

        [Required(ErrorMessage = "El ID del administrador es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del administrador debe ser un número válido")]
        int AdminId,

        [DataType(DataType.DateTime)]
        DateTime? ActionDate = null
    );
}
