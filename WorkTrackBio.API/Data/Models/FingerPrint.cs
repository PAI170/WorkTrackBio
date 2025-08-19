using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class FingerPrint
    {
        public int Id { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public byte[] TemplateFingerPrint { get; set; } = Array.Empty<byte>();
        
        [Required]
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        [ForeignKey("EmployeeId")]
        public virtual EmployeeInfo Employee { get; set; } = null!;
    }
}
