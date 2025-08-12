using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class Projects : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [ForeignKey("States")]
        public int StateId { get; set; }

        // Navigation Properties
        public virtual States State { get; set; } = null!;
        public virtual ICollection<Assistance> Assistances { get; set; } = new List<Assistance>();
        public virtual ICollection<ProjectsAssigns> ProjectsAssigns { get; set; } = new List<ProjectsAssigns>();
        public virtual ICollection<ProjectMaintenance> ProjectMaintenances { get; set; } = new List<ProjectMaintenance>();
        public virtual ICollection<ProjectWarranty> ProjectWarranties { get; set; } = new List<ProjectWarranty>();

        // Implementación de métodos abstractos para auditoría
        public override string GetEntityId() => Id.ToString();
        public override string GetEntityType() => "Project";
    }
}