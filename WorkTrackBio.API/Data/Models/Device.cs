using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class Device
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DeviceId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;
        
        [Required]
        public int ProjectId { get; set; }
        
        [StringLength(200)]
        public string? Location { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DeviceType { get; set; } = "Fingerprint";
        
        [StringLength(100)]
        public string? IpAddress { get; set; }
        
        [StringLength(20)]
        public string? SerialNumber { get; set; }
        
        [StringLength(50)]
        public string? FirmwareVersion { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastActivity { get; set; }
        
        // Navigation property
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;
    }
}
