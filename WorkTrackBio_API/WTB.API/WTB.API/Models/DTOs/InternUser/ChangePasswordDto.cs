using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO para cambio de contraseña de usuario administrativo
    /// </summary>
    public record ChangePasswordDto(
        [Required]
        int UserId,

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña actual es obligatoria")]
        string CurrentPassword,

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La nueva contraseña debe tener entre 8 y 100 caracteres")]
        string NewPassword,

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Debe confirmar la nueva contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        string ConfirmNewPassword
    );
}
