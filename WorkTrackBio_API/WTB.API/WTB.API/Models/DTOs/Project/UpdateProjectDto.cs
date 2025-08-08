using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Project
{
    /// <summary>
    /// DTO para actualizar un proyecto existente
    /// </summary>
    public record UpdateProjectDto(
        [Required]
        int Id,

        [Required]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre del proyecto debe tener entre 3 y 150 caracteres")]
        string ProjectName,

        DateTime? StartDate,

        DateTime? EndDate,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido")]
        int StateId
    );
}
