using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorkTrackBio.API.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string IdentificationNumber { get; set; } = string.Empty;

        [StringLength(150)]
        public string ContactName { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string PaymentCondition { get; set; } = string.Empty;

        [StringLength(3)]
        public string Currency { get; set; } = "CRC";

        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public ExonerationData Exoneration { get; set; } = new ExonerationData();

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }

    [Owned]
    public class ExonerationData
    {
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateTime? IssueDate { get; set; }
    }

}