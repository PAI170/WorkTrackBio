using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WTB.API.Models.Entities
{
    /// <summary>
    /// Entidad para registrar auditoría automática de todos los cambios del sistema
    /// </summary>
    public class AuditRegister
    {
        [Key]
        public int Id { get; set; }

        // Información de la entidad modificada
        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = string.Empty; // "Employee", "Project", "Assistance", etc.

        [Required]
        public string EntityId { get; set; } = string.Empty; // ID de la entidad (puede ser string para flexibilidad)

        // Información de la acción
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty; // "CREATE", "UPDATE", "DELETE"

        // Detalles del cambio
        [StringLength(100)]
        public string? PropertyName { get; set; } // Campo específico que cambió

        [StringLength(500)]
        public string? OldValue { get; set; } // Valor anterior

        [StringLength(500)]
        public string? NewValue { get; set; } // Nuevo valor

        [Required]
        public string DetailChange { get; set; } = string.Empty; // Descripción general del cambio

        // Metadatos
        [Required]
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int AdminId { get; set; } // Quién realizó el cambio

        // Campo opcional para casos especiales de asistencia
        public int? AssistanceId { get; set; } // Opcional, solo para casos especiales

        // Navigation Properties
        public virtual InternUsers Admin { get; set; } = null!;
        public virtual Assistance? Assistance { get; set; } // Opcional
    }
}
