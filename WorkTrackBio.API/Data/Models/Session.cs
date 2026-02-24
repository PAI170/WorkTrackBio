using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class Session
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string TokenId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string RefreshToken { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? DeviceInfo { get; set; }
        
        [StringLength(45)]
        public string? IpAddress { get; set; }
        
        [StringLength(500)]
        public string? UserAgent { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime ExpiresAt { get; set; }
        
        public DateTime? LastActivity { get; set; }
        
        public DateTime? LoggedOutAt { get; set; }
        
        [ForeignKey("UserId")]
        public virtual InternUser User { get; set; } = null!;
    }
}
