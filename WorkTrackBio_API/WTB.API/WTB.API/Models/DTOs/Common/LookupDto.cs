namespace WTB.API.Models.DTOs.Common
{
    /// <summary>
    /// DTO genérico para valores de lookup/catálogo
    /// </summary>
    public record LookupDto(
        int Id,
        string Name,
        string? Description = null
    );

    /// <summary>
    /// DTO específico para estados del sistema
    /// </summary>
    public record StateDto(
        int Id,
        string StateName,
        string StateType,
        string? Description = null
    );

    /// <summary>
    /// DTO específico para roles de usuario
    /// </summary>
    public record RoleDto(
        int Id,
        string RoleName,
        string? Description = null
    );

    /// <summary>
    /// DTO específico para tipos de documento
    /// </summary>
    public record DocumentTypeDto(
        int Id,
        string DocumentName,
        string? Description = null
    );
}
