using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class TravelExpense
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        [Required]
        public DateOnly ExpenseDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ExpenseType { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(500)]
        public string ReceiptUrl { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Notes { get; set; }

        [Required]
        public int StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;

        public int? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public virtual EmployeeInfo? ApprovedBy { get; set; }
    }
}