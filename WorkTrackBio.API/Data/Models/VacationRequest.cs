using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class VacationRequest : BaseEntity
    {
        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        public int TotalDays { get; set; }

        [Required]
        public VacationStatus Status { get; set; } = VacationStatus.Pending;

        [StringLength(255)]
        public string? Reason { get; set; }

        [StringLength(255)]
        public string? RejectionReason { get; set; }

        public int? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public virtual AppUser? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }
    }
}
