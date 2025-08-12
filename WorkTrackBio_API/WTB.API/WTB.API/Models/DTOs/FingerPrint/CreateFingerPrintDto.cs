using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.FingerPrint
{
    /// <summary>
    /// DTO para registrar una nueva huella dactilar
    /// </summary>
    public record CreateFingerPrintDto(
        [Required(ErrorMessage = "El ID del empleado es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser un número válido")]
        int EmployeeId,

        [Required(ErrorMessage = "El template de huella es requerido")]
        byte[] TemplateFingerPrint,

        [DataType(DataType.DateTime)]
        DateTime? IssueDate = null
    );
}
