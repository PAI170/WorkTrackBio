using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class EmployeeInfo
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;
        
        [Required]
        public int DocumentTypeId { get; set; }
        
        public DateOnly? DocumentExpire { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        
        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        
        [StringLength(20)]
        public string? EmergencyContactPhoneNumber { get; set; }
        
        [Required]
        public DateOnly Birthday { get; set; }
        
        [Required]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? CostPerHour { get; set; }
        
        [Required]
        public int StateId { get; set; }
        
        [StringLength(255)]
        public string? Address { get; set; }
        
        [StringLength(50)]
        public string? IBAN { get; set; }
        
        // Navigation properties
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; } = null!;
        
        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;
    }
}
