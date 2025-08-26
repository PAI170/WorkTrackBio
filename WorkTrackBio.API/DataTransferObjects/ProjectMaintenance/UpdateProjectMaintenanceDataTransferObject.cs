using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.DataTransferObjects.ProjectMaintenance
{
    public class UpdateProjectMaintenanceDataTransferObject
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID debe ser mayor que 0")]
        public int Id { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "El ID del proyecto debe ser mayor que 0")]
        public int? IdProject { get; set; }
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? MaintenanceDescription { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser mayor que 0")]
        public int? MadeById { get; set; }
        
        [Range(0, 999999.99, ErrorMessage = "El costo debe estar entre 0 y 999,999.99")]
        public decimal? MaintenanceCost { get; set; }
        
        [StringLength(255, ErrorMessage = "La información adicional no puede exceder 255 caracteres")]
        public string? AdditionalInfo { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "El ID del estado debe ser mayor que 0")]
        public int? StateId { get; set; }
    }
}
