namespace WTB.API.Models.DTOs.Employee
{
    /// <summary>
    /// DTO simplificado para listados de empleados
    /// </summary>
    public record EmployeeListDto(
        int Id,
        string DocumentNumber,
        string FullName,
        string? PhoneNumber,
        string DocumentTypeName,
        string StateName,
        decimal? CostPerHour,
        DateTime RegisterDate,
        bool IsActive
    );
}
