using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.ProjectAssign
{
    /// <summary>
    /// DTO para asignar un empleado a un proyecto
    /// </summary>
    public record CreateProjectAssignDto(
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




