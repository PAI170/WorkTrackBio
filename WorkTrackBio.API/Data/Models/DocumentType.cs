using System.ComponentModel.DataAnnotations;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class DocumentType : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string DocumentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public virtual ICollection<EmployeeInfo> EmployeeInfos { get; set; } = new List<EmployeeInfo>();
    }
}
