using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO para actualizar un usuario administrativo
    /// </summary>
    public record UpdateInternUserDto(
        [Required]
        int Id,

        [Required]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        string Email,

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
        string FirstName,

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
        string LastName,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol válido")]
        int RolId,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido")]
        int StateId
    );
}






