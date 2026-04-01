using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class Assistance : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalHours { get; set; }

        [Required]
        [StringLength(50)]
        public string RegisterType { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateOnly CheckInDateOnly { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual EmployeeInfo Employee { get; set; } = null!;

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;
    }
}
