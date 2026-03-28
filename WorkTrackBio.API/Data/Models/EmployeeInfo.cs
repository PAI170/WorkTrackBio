using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkTrackBio.API.Data.Models
{
    public class EmployeeInfo
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string PersonalEmail { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public DateOnly Birthday { get; set; }

        [Required]
        public DateOnly HireDate { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? IBAN { get; set; }

        [Required]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [StringLength(255)]
        public string? PhotoUrl { get; set; }

        public int StateId { get; set; }
        public virtual State State { get; set; } = null!;

        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; } = null!;

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly DocumentExpire { get; set; }

        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [StringLength(50)]
        public string Nationality { get; set; } = string.Empty;

        [StringLength(50)]
        public string MaritalStatus { get; set; } = string.Empty;

        [Range(0, 20)]
        public int NumberOfChildren { get; set; }

        [StringLength(100)]
        public string? SpouseName { get; set; }

        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhoneNumber { get; set; }

        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

        public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

        public virtual ICollection<Assistance> Assistances { get; set; } = new List<Assistance>();

        public virtual ICollection<EmployeeDocument> EmployeeDocuments { get; set; } = new List<EmployeeDocument>();

        public bool CanClaimExpenses { get; set; } = false;

        public SalaryData Salary { get; set; } = new SalaryData();

        public ExitData ExitInformation { get; set; } = new ExitData();
    }

    [Owned]
    public class SalaryData
    {
        [StringLength(50)]
        public string PaymentFrequency { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CostPerHour { get; set; }

        [StringLength(3)]
        public string Currency { get; set; } = "CRC";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? FixedSalary { get; set; }

        public int AvailableVacationDays { get; set; }
        public int UsedVacationDays { get; set; }
    }

    [Owned]
    public class ExitData
    {
        public DateOnly? ExitDate { get; set; }

        [StringLength(255)]
        public string? ExitReason { get; set; }
        public bool IsRehire { get; set; } = false;
        public DateOnly? LiquidationDate { get; set; }
        public DateOnly? RehireDate { get; set; }
    }

}