using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.Data.Models
{
    public class State
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string StateName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string StateType { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? Description { get; set; }
    }
}
