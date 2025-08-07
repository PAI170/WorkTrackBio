using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class ProjectsAssigns
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeInfo")]
        public int EmployeeId { get; set; }

        [Required]
        [ForeignKey("Projects")]
        public int ProjectId { get; set; }

        [Required]
        public DateTime AssignDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        // Navigation Properties
        public virtual EmployeeInfo Employee { get; set; } = null!;
        public virtual Projects Project { get; set; } = null!;
    }
}