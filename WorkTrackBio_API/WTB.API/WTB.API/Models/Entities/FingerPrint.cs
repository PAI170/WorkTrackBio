using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class FingerPrint : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeInfo")]
        public int EmployeeId { get; set; }

        [Required]
        public byte[] TemplateFingerPrint { get; set; } = Array.Empty<byte>();

        [Required]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual EmployeeInfo Employee { get; set; } = null!;

        // Implementación de métodos abstractos para auditoría
        public override string GetEntityId() => Id.ToString();
        public override string GetEntityType() => "FingerPrint";
    }
}