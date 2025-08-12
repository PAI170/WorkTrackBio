using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    /// <summary>
    /// Entidad para gestionar dispositivos lectores de huella por proyecto
    /// </summary>
    public class Device : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DeviceId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Projects")]
        public int ProjectId { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string DeviceType { get; set; } = "Fingerprint";

        [Required]
        public bool IsActive { get; set; } = true;

        public DateTime? LastSeen { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Configuración específica del dispositivo
        [StringLength(100)]
        public string? IpAddress { get; set; }

        [StringLength(20)]
        public string? SerialNumber { get; set; }

        [StringLength(50)]
        public string? FirmwareVersion { get; set; }

        // Navigation Properties
        public virtual Projects Project { get; set; } = null!;
        public virtual ICollection<Assistance> Assistances { get; set; } = new List<Assistance>();

        // Implementación de métodos abstractos para auditoría
        public override string GetEntityId() => Id.ToString();
        public override string GetEntityType() => "Device";
    }
}
