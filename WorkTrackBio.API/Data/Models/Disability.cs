using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class Disability : BaseEntity
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
        public int EmployerPaidDays { get; set; }

        [Required]
        public int CCSSPaidDays { get; set; }

        [Required]
        public DisabilityType DisabilityType { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? CSSDocumentNumber { get; set; }
    }
}
