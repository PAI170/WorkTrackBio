namespace WTB.API.Models.DTOs.Assistance
{
    /// <summary>
    /// DTO simplificado para listados de asistencias
    /// </summary>
    public record AssistanceListDto(
        int Id,
        string EmployeeName,
        string ProjectName,
        DateTime CheckIn,
        DateTime? CheckOut,
        decimal? TotalHours,
        string RegisterType,
        string Status,
        bool IsActive,
        bool IsOvertime
    );
}
