using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO para actualizar una asignación de empleado a proyecto
    /// </summary>
    public record UpdateProjectAssignDto(
        [Required]
        int Id,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado válido")]
        int EmployeeId,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un proyecto válido")]
        int ProjectId,

        [Required]
        DateTime AssignDate,

        DateTime? EndDate = null
    );
}




