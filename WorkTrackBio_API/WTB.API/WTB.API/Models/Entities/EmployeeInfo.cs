using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class EmployeeInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [ForeignKey("DocumentType")]
        public int DocumentTypeId { get; set; }

        public DateTime? DocumentExpire { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhoneNumber { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CostPerHour { get; set; }

        [Required]
        [ForeignKey("States")]
        public int StateId { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? IBAN { get; set; }

        // Navigation Properties
        public virtual DocumentType DocumentType { get; set; } = null!;
        public virtual States State { get; set; } = null!;
        public virtual ICollection<Assistance> Assistances { get; set; } = new List<Assistance>();
        public virtual ICollection<ProjectsAssigns> ProjectsAssigns { get; set; } = new List<ProjectsAssigns>();
        public virtual ICollection<FingerPrint> FingerPrints { get; set; } = new List<FingerPrint>();
        public virtual ICollection<ProjectMaintenance> ProjectMaintenancesMadeBy { get; set; } = new List<ProjectMaintenance>();
        public virtual ICollection<ProjectWarranty> ProjectWarrantiesMadeBy { get; set; } = new List<ProjectWarranty>();
    }
}