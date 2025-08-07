using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class States
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string StateName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string StateType { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<InternUsers> InternUsers { get; set; } = new List<InternUsers>();
        public virtual ICollection<Projects> Projects { get; set; } = new List<Projects>();
        public virtual ICollection<EmployeeInfo> EmployeeInfo { get; set; } = new List<EmployeeInfo>();
        public virtual ICollection<ProjectMaintenance> ProjectMaintenances { get; set; } = new List<ProjectMaintenance>();
        public virtual ICollection<ProjectWarranty> ProjectWarranties { get; set; } = new List<ProjectWarranty>();
    }
}