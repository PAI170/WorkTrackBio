using System.ComponentModel.DataAnnotations;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class Department : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        public virtual ICollection<EmployeeInfo> EmployeeInfos { get; set; } = new List<EmployeeInfo>();
    }
}
