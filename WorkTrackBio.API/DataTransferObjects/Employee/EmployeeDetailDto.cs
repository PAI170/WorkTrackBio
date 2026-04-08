namespace WorkTrackBio.API.DataTransferObjects.Employee
{
    public class EmployeeDetailDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalEmail { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateOnly Birthday { get; set; }
        public DateOnly HireDate { get; set; }
        public string? Address { get; set; }
        public string? IBAN { get; set; }
        public bool IsActive { get; set; }
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
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public SalaryDataDto Salary { get; set; } = new();
        public ExitDataDto ExitInformation { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SalaryDataDto
    {
        public string PaymentFrequency { get; set; } = string.Empty;
        public decimal? CostPerHour { get; set; }
        public string Currency { get; set; } = "CRC";
        public decimal? FixedSalary { get; set; }
        public int AvailableVacationDays { get; set; }
        public int UsedVacationDays { get; set; }
    }

    public class ExitDataDto
    {
        public DateOnly? ExitDate { get; set; }
        public string? ExitReason { get; set; }
        public bool IsRehire { get; set; }
        public DateOnly? LiquidationDate { get; set; }
        public DateOnly? RehireDate { get; set; }
    }
}