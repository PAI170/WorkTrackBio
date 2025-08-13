using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO para registrar check-out de un empleado
    /// </summary>
    public record CheckOutDto(
        [Required]
        int AssistanceId,

        [Required]
        DateTime CheckOut,

        [StringLength(500, ErrorMessage = "Las notas no pueden exceder 500 caracteres")]
        string? Notes
    );
}

