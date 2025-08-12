using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.ProjectWarranty
{
    /// <summary>
    /// DTO para crear un nuevo registro de garantía de proyecto
    /// </summary>
    public record CreateProjectWarrantyDto(
        [Required(ErrorMessage = "El ID del proyecto es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del proyecto debe ser un número válido")]
        int IdProject,

        [Required(ErrorMessage = "La descripción de la garantía es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        string WarrantyDescription,

        [Required(ErrorMessage = "El ID del empleado que registra la garantía es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser un número válido")]
        int MadeById,

        [Required(ErrorMessage = "El ID del estado es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del estado debe ser un número válido")]
        int StateId,

        [Range(0, 999999.99, ErrorMessage = "El costo debe estar entre 0 y 999999.99")]
        decimal? WarrantyCost = null,

        [DataType(DataType.DateTime)]
        DateTime? WarrantyDate = null
    );
}
