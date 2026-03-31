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
        public StateType StateType { get; set; }

        [StringLength(50)]
        public string? Description { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<EmployeeInfo> EmployeeInfos { get; set; } = new List<EmployeeInfo>();
        public virtual ICollection<ProjectMaintenance> ProjectMaintenances { get; set; } = new List<ProjectMaintenance>();
        public virtual ICollection<ProjectWarranty> ProjectWarranties { get; set; } = new List<ProjectWarranty>();
        public virtual ICollection<TravelExpense> TravelExpenses { get; set; } = new List<TravelExpense>();
    }
}