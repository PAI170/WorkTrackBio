namespace WTB.API.Models.DTOs.Employee
{
    /// <summary>
    /// DTO para respuesta con información completa del empleado
    /// </summary>
    public record EmployeeResponseDto(
        int Id,
        string DocumentNumber,
        int DocumentTypeId,
        string DocumentTypeName,
        DateTime? DocumentExpire,
        string FirstName,
        string LastName,
        string FullName,
        string? PhoneNumber,
        string? EmergencyContact,
        string? EmergencyContactPhoneNumber,
        DateTime Birthday,
        int Age,
        DateTime RegisterDate,
        decimal? CostPerHour,
        int StateId,
        string StateName,
        string? Address,
        string? IBAN,
        bool HasFingerPrint,
        int TotalProjects,
        DateTime? LastAssistance
    );
}
