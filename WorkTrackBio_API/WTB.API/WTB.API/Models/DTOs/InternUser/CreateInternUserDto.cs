using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO para crear un nuevo usuario administrativo
    /// </summary>
    public record CreateInternUserDto(
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
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        string Password,

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Debe confirmar la contraseña")]
        string ConfirmPassword,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol válido")]
        int RolId
    );
}




