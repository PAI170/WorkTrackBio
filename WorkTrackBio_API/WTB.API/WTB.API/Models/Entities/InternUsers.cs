using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class InternUsers : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PasswordSalt { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Roles")]
        public int RolId { get; set; }

        public DateTime? LastLogin { get; set; }

        [Required]
        [ForeignKey("States")]
        public int StateId { get; set; }

        // Navigation Properties
        public virtual Roles Role { get; set; } = null!;
        public virtual States State { get; set; } = null!;
        public virtual ICollection<AuditRegister> AuditRegisters { get; set; } = new List<AuditRegister>();

        // Implementación de métodos abstractos para auditoría
        public override string GetEntityId() => Id.ToString();
        public override string GetEntityType() => "InternUser";
    }
}