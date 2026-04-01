using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class ChristmasBonus : BaseEntity
    {
        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        public int Year { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalEarned { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SuggestedAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalAmount { get; set; }

        public DateOnly? PaymentDate { get; set; }

        public bool IsConfirmed { get; set; } = false;

        [StringLength(255)]
        public string? Notes { get; set; }

        public int? ConfirmedById { get; set; }
        [ForeignKey("ConfirmedById")]
        public virtual EmployeeInfo? ConfirmedBy { get; set; }

        public virtual ICollection<ChristmasBonusDetail> Details { get; set; } = new List<ChristmasBonusDetail>();
    }
}
