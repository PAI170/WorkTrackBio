using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class AuditRegister
    {
        public int Id { get; set; }
        
        [Required]
        public int AssistanceId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;
        
        [Required]
        public string DetailChange { get; set; } = string.Empty;
        
        [Required]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int AdminId { get; set; }
        
        // Navigation properties
        [ForeignKey("AssistanceId")]
        public virtual Assistance Assistance { get; set; } = null!;
        
        [ForeignKey("AdminId")]
        public virtual InternUser Admin { get; set; } = null!;
    }
}
