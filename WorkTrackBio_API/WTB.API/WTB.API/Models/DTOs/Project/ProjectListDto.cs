namespace WTB.API.Models.DTOs.Project
{
    /// <summary>
    /// DTO simplificado para listados de proyectos
    /// </summary>
    public record ProjectListDto(
        int Id,
        string ProjectName,
        DateTime? StartDate,
        DateTime? EndDate,
        string StateName,
        bool IsActive,
        bool IsOverdue,
        int TotalEmployees,
        decimal TotalHoursWorked,
        decimal? ProgressPercentage
    );
}
