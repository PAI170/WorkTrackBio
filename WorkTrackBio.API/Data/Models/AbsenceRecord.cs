using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class AbsenceRecord
    {
        public int Id { get; set; }

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

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? RegisteredById { get; set; }
        [ForeignKey("RegisteredById")]
        public virtual EmployeeInfo? RegisteredBy { get; set; }
    }
} 