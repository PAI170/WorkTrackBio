using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class AuditRegister
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Assistance")]
        public int AssistanceId { get; set; }

        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;

        [Required]
        public string DetailChange { get; set; } = string.Empty;

        [Required]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("InternUsers")]
        public int AdminId { get; set; }

        // Navigation Properties
        public virtual Assistance Assistance { get; set; } = null!;
        public virtual InternUsers Admin { get; set; } = null!;
    }
}
