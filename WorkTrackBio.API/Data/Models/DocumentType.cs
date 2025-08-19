using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.Data.Models
{
    public class DocumentType
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string DocumentName { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Description { get; set; }
    }
}
