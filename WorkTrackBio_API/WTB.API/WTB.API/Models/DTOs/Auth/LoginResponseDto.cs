namespace WTB.API.Models.DTOs.Auth
{
    /// <summary>
    /// DTO para respuesta de inicio de sesión exitoso
    /// </summary>
    public record LoginResponseDto(
        string Token,
        string RefreshToken,
        DateTime ExpiresAt,
        UserInfoDto User
    );

    /// <summary>
    /// DTO con información básica del usuario autenticado
    /// </summary>
    public record UserInfoDto(
        int Id,
        string Email,
        string FullName,
        string RoleName,
        bool IsActive,
        ICollection<string> Permissions
    );
}




