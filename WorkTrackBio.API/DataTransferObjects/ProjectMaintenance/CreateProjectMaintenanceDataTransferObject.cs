using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.DataTransferObjects.ProjectMaintenance
{
    public class CreateProjectMaintenanceDataTransferObject
    {
        [Required(ErrorMessage = "El ID del proyecto es obligatorio")]
        public int IdProject { get; set; }
        
        [Required(ErrorMessage = "La descripción del mantenimiento es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string MaintenanceDescription { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El ID del empleado que realizó el mantenimiento es obligatorio")]
        public int MadeById { get; set; }
        
        [Range(0, 999999.99, ErrorMessage = "El costo debe estar entre 0 y 999,999.99")]
        public decimal? MaintenanceCost { get; set; }
        
        [StringLength(255, ErrorMessage = "La información adicional no puede exceder 255 caracteres")]
        public string? AdditionalInfo { get; set; }
        
        [Required(ErrorMessage = "El ID del estado es obligatorio")]
        public int StateId { get; set; }
    }
}
