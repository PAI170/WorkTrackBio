namespace WTB.API.Models.DTOs.InternUser
{
    /// <summary>
    /// DTO simplificado para listados de usuarios administrativos
    /// </summary>
    public record InternUserListDto(
        int Id,
        string Email,
        string FullName,
        string RoleName,
        string StateName,
        bool IsActive,
        DateTime CreationDate,
        DateTime? LastLogin,
        string Status
    );
}




