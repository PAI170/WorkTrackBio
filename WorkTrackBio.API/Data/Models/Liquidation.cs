using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class Liquidation : BaseEntity
    {
        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        public LiquidationStatus Status { get; set; } = LiquidationStatus.Draft;

        [Column(TypeName = "decimal(10,2)")]
        public decimal SuggestedNoticePay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalNoticePay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SuggestedSeverancePay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalSeverancePay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SuggestedVacationPay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalVacationPay { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SuggestedChristmasBonus { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalChristmasBonus { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SuggestedTotal { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalTotal { get; set; }

        public DateOnly? PaymentDate { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        public int? ConfirmedById { get; set; }
        [ForeignKey("ConfirmedById")]
        public virtual EmployeeInfo? ConfirmedBy { get; set; }

        public DateTime? ConfirmedAt { get; set; }
    }
}
