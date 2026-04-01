using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class PayrollRun : BaseEntity
    {
        [Required]
        public PayFrequency PayFrequency { get; set; }

        [Required]
        public DateOnly PeriodStart { get; set; }

        [Required]
        public DateOnly PeriodEnd { get; set; }

        public DateOnly? PaymentDate { get; set; }

        [Required]
        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

        [StringLength(255)]
        public string? Notes { get; set; }

        [Required]
        public int CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual AppUser CreatedBy { get; set; } = null!;

        public DateTime? ApprovedAt { get; set; }

        public int? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public virtual AppUser? ApprovedBy { get; set; }

        public virtual ICollection<PayrollEntry> PayrollEntries { get; set; } = new List<PayrollEntry>();
    }
}
