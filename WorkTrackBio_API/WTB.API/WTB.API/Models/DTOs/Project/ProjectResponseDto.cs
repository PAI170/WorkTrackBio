namespace WTB.API.Models.DTOs.Project
{
    /// <summary>
    /// DTO para respuesta con información básica del proyecto
    /// </summary>
    public record ProjectResponseDto(
        int Id,
        string ProjectName,
        DateTime? StartDate,
        DateTime? EndDate,
        int StateId
    );
}
