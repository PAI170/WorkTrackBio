using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.Data.Models
{
    public class State
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string StateName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string StateType { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Description { get; set; }
        
        // Navigation Properties - Relaciones inversas
        public virtual ICollection<InternUser> InternUsers { get; set; } = new List<InternUser>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<EmployeeInfo> EmployeeInfos { get; set; } = new List<EmployeeInfo>();
        public virtual ICollection<ProjectMaintenance> ProjectMaintenances { get; set; } = new List<ProjectMaintenance>();
        public virtual ICollection<ProjectWarranty> ProjectWarranties { get; set; } = new List<ProjectWarranty>();
    }
}
