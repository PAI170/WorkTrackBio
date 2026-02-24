using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class Project
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; } = string.Empty;
        
        public DateOnly? StartDate { get; set; }
        
        public DateOnly? EndDate { get; set; }
        
        [Required]
        public int StateId { get; set; }
        
        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;
    }
}
