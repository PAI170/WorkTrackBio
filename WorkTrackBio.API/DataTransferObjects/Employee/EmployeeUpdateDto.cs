using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.DataTransferObjects.Employee
{
    public class EmployeeUpdateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateOnly Birthday { get; set; }
        public DateOnly HireDate { get; set; }
        public string? Address { get; set; }
        public string? IBAN { get; set; }
        public string? PhotoUrl { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateOnly DocumentExpire { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string MaritalStatus { get; set; } = string.Empty;
        public int NumberOfChildren { get; set; }
        public string? SpouseName { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyContactPhoneNumber { get; set; }
        public bool CanClaimExpenses { get; set; }
        public bool IsActive { get; set; }
        public int StateId { get; set; }
        public int DocumentTypeId { get; set; }
        public int DepartmentId { get; set; }
        public SalaryCreateDto Salary { get; set; } = new();
        public ExitUpdateDto ExitInformation { get; set; } = new();
    }

    public class ExitUpdateDto
    {
        public DateOnly? ExitDate { get; set; }
        public string? ExitReason { get; set; }
        public bool IsRehire { get; set; }
        public DateOnly? LiquidationDate { get; set; }
        public DateOnly? RehireDate { get; set; }
    }
}