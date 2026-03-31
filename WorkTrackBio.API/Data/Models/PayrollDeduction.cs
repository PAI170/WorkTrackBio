using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class PayrollDeduction
    {
        public int Id { get; set; }

        [Required]
        public int PayrollId { get; set; }

        [ForeignKey("PayrollId")]
        public virtual PayrollEntry Payroll { get; set; } = null!;

        public int? EmployeeDeductionId { get; set; }

        [ForeignKey("EmployeeDeductionId")]
        public virtual EmployeeDeduction? EmployeeDeduction { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }


    }
}