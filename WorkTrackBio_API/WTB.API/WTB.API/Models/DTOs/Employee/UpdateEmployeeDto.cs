using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Employee
{
    /// <summary>
    /// DTO para actualizar un empleado existente
    /// </summary>
    public record UpdateEmployeeDto(
        [Required]
        int Id,

        [Required]
        [StringLength(50, ErrorMessage = "El número de documento no puede exceder 50 caracteres")]
        string DocumentNumber,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de documento válido")]
        int DocumentTypeId,

        DateTime? DocumentExpire,

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
        string FirstName,

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres")]
        string LastName,

        [StringLength(20, ErrorMessage = "El número de teléfono no puede exceder 20 caracteres")]
        string? PhoneNumber,

        [StringLength(100, ErrorMessage = "El contacto de emergencia no puede exceder 100 caracteres")]
        string? EmergencyContact,

        [StringLength(20, ErrorMessage = "El teléfono de emergencia no puede exceder 20 caracteres")]
        string? EmergencyContactPhoneNumber,

        [Required]
        DateTime Birthday,

        [Range(0, double.MaxValue, ErrorMessage = "El costo por hora debe ser un valor positivo")]
        decimal? CostPerHour,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido")]
        int StateId,

        [StringLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres")]
        string? Address,

        [StringLength(50, ErrorMessage = "El IBAN no puede exceder 50 caracteres")]
        string? IBAN
    );
}
