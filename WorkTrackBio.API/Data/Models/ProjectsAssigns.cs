using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class ProjectsAssigns
    {
        public int Id { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public int ProjectId { get; set; }
        
        [Required]
        public DateTime AssignDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? EndDate { get; set; }
        
        [ForeignKey("EmployeeId")]
        public virtual EmployeeInfo Employee { get; set; } = null!;
        
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;
    }
}
