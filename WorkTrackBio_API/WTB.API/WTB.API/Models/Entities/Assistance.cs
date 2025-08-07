using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    public class Assistance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("EmployeeInfo")]
        public int EmployeeId { get; set; }

        [Required]
        [ForeignKey("Projects")]
        public int ProjectId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? TotalHours { get; set; }

        [Required]
        [StringLength(50)]
        public string RegisterType { get; set; } = string.Empty;

        // Computed column - se calcula automáticamente en la base de datos
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CheckInDateOnly { get; set; }

        // Navigation Properties
        public virtual EmployeeInfo Employee { get; set; } = null!;
        public virtual Projects Project { get; set; } = null!;
        public virtual ICollection<AuditRegister> AuditRegisters { get; set; } = new List<AuditRegister>();
    }
}