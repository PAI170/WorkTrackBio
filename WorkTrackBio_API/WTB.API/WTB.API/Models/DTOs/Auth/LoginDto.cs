using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.DTOs.Auth
{
    /// <summary>
    /// DTO para inicio de sesión
    /// </summary>
    public record LoginDto(
        [Required]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        string Email,

        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "La contraseña es obligatoria")]
        string Password,

        bool RememberMe = false
    );
}




