using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class UserSession
    {
        public int Id { get; set; }

        [Required]
        public int AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public virtual AppUser AppUser { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string JwtId { get; set; } = string.Empty;

        public bool IsUsed { get; set; } = false;

        public bool IsRevoked { get; set; } = false;

        [Required]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}