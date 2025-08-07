using System.ComponentModel.DataAnnotations;

namespace WTB.API.Models.Entities
{
    public class DocumentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<EmployeeInfo> EmployeeInfo { get; set; } = new List<EmployeeInfo>();
    }
}