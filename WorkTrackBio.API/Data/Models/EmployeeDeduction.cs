using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class EmployeeDeduction
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string DeductionName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? FixedAmount { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Percentage { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<PayrollDeduction> PayrollDeductions { get; set; } = new List<PayrollDeduction>();
    }
}