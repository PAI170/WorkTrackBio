using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class PayrollEntry : BaseEntity
    {
        [Required]
        public int PayrollRunId { get; set; }
        [ForeignKey("PayrollRunId")]
        public virtual PayrollRun PayrollRun { get; set; } = null!;

        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? RegularHours { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? OvertimeHours { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? OvertimeAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal GrossSalary { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalDeductions { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal NetSalary { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "CRC";

        [StringLength(255)]
        public string? Notes { get; set; }

        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } = new List<PayrollDeduction>();
    }
}
