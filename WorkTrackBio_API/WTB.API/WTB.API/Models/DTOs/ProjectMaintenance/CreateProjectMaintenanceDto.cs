using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.ProjectMaintenance
{
    /// <summary>
    /// DTO para crear un nuevo registro de mantenimiento de proyecto
    /// </summary>
    public record CreateProjectMaintenanceDto(
        [Required(ErrorMessage = "El ID del proyecto es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del proyecto debe ser un número válido")]
        int IdProject,

        [Required(ErrorMessage = "La descripción del mantenimiento es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        string MaintenanceDescription,

        [Required(ErrorMessage = "El ID del empleado que realiza el mantenimiento es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser un número válido")]
        int MadeById,

        [Required(ErrorMessage = "El ID del estado es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del estado debe ser un número válido")]
        int StateId,

        [Range(0, 999999.99, ErrorMessage = "El costo debe estar entre 0 y 999999.99")]
        decimal? MaintenanceCost = null,

        [StringLength(255, ErrorMessage = "La información adicional no puede exceder 255 caracteres")]
        string? AdditionalInfo = null,

        [DataType(DataType.DateTime)]
        DateTime? MaintenanceDate = null
    );
}
