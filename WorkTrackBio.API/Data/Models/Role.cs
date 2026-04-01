using System.ComponentModel.DataAnnotations;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class Role : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
    }
}
