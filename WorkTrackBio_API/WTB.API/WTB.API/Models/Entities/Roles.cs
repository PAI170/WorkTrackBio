using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.Entities
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<InternUsers> InternUsers { get; set; } = new List<InternUsers>();
    }
}