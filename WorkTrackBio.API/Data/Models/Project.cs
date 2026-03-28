using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = string.Empty;

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }

        [StringLength(250)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public int StateId { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;

        public int? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        public virtual ICollection<Assistance> Assistances { get; set; } = new List<Assistance>();
        public virtual ICollection<TravelExpense> TravelExpenses { get; set; } = new List<TravelExpense>();
        public virtual ICollection<ProjectMaintenance> ProjectMaintenances { get; set; } = new List<ProjectMaintenance>();
        public virtual ICollection<ProjectWarranty> ProjectWarranties { get; set; } = new List<ProjectWarranty>();
    }
}