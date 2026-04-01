using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class AbsenceRecord : BaseEntity
    {
        [Required]
        public int EmployeeInfoId { get; set; }
        [ForeignKey("EmployeeInfoId")]
        public virtual EmployeeInfo EmployeeInfo { get; set; } = null!;

        [Required]
        public DateOnly AbsenceDate { get; set; }

        [Required]
        public AbsenceType AbsenceType { get; set; }

        [StringLength(255)]
        public string? Reason { get; set; }

        public int? VacationRequestId { get; set; }
        [ForeignKey("VacationRequestId")]
        public virtual VacationRequest? VacationRequest { get; set; }

        public int? RegisteredById { get; set; }
        [ForeignKey("RegisteredById")]
        public virtual EmployeeInfo? RegisteredBy { get; set; }
    }
}
