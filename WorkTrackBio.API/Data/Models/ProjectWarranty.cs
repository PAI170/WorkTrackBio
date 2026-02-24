using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTrackBio.API.Data.Models
{
    public class ProjectWarranty
    {
        public int Id { get; set; }
        
        [Required]
        public int IdProject { get; set; }
        
        [Required]
        public DateTime WarrantyDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public string WarrantyDescription { get; set; } = string.Empty;
        
        [Required]
        public int MadeById { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? WarrantyCost { get; set; }
        
        [Required]
        public int StateId { get; set; }
        
        [ForeignKey("IdProject")]
        public virtual Project Project { get; set; } = null!;
        
        [ForeignKey("MadeById")]
        public virtual EmployeeInfo MadeBy { get; set; } = null!;
        
        [ForeignKey("StateId")]
        public virtual State State { get; set; } = null!;
    }
}
