using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class InternUser
    {
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
        public int RolId { get; set; }
        
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int StateId { get; set; }
        
        public DateTime? LastLogin { get; set; }

        [StringLength(50)]
        public string? DocumentNumber { get; set; }
        
        public int? DocumentTypeId { get; set; }
        
        public DateOnly? DocumentExpire { get; set; }
        
        [ForeignKey("RolId")]
        public virtual Role Role { get; set; } = null!;
        
        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;
        
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType? DocumentType { get; set; }
    }
}
