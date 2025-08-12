using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class ProjectMaintenance : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Projects")]
        public int IdProject { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string MaintenanceDescription { get; set; } = string.Empty;

        [Required]
        [ForeignKey("EmployeeInfo")]
        public int MadeById { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? MaintenanceCost { get; set; }

        [StringLength(255)]
        public string? AdditionalInfo { get; set; }

        [Required]
        [ForeignKey("States")]
        public int StateId { get; set; }

        // Navigation Properties
        public virtual Projects Project { get; set; } = null!;
        public virtual EmployeeInfo MadeBy { get; set; } = null!;
        public virtual States State { get; set; } = null!;

        // Implementación de métodos abstractos para auditoría
        public override string GetEntityId() => Id.ToString();
        public override string GetEntityType() => "ProjectMaintenance";
    }
}