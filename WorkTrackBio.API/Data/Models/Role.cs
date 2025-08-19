using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.Data.Models
{
    public class Role
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Description { get; set; }
    }
}
