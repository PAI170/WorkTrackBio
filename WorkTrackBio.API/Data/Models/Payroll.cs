using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class Payroll
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeInfoId { get; set; }

        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal GrossSalary { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalDeductions { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal NetSalary { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        public int StateId { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;

        [StringLength(255)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual AppUser CreatedBy { get; set; } = null!;

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public virtual AppUser? ApprovedBy { get; set; }

        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } = new List<PayrollDeduction>();
    }
}