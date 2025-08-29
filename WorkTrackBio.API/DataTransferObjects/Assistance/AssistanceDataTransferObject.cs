namespace WorkTrackBio.API.DataTransferObjects.Assistance
{
    /// <summary>
    /// DTO principal para la respuesta de Assistance
    /// Incluye información completa del registro de asistencia
    /// </summary>
    public class AssistanceDataTransferObject
    {
        public int Id { get; set; }
        
        /// <summary>
        /// ID del empleado que registra la asistencia
        /// </summary>
        public int EmployeeId { get; set; }
        
        /// <summary>
        /// Nombre completo del empleado
        /// </summary>
        public string EmployeeName { get; set; } = string.Empty;
        
        /// <summary>
        /// ID del proyecto en el que trabaja
        /// </summary>
        public int ProjectId { get; set; }
        
        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha y hora de creación del registro
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Fecha y hora de la última modificación
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        
        /// <summary>
        /// Notas adicionales del registro
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Fecha y hora de entrada
        /// </summary>
        public DateTime CheckIn { get; set; }
        
        /// <summary>
        /// Fecha y hora de salida
        /// </summary>
        public DateTime? CheckOut { get; set; }
        
        /// <summary>
        /// Total de horas trabajadas
        /// </summary>
        public decimal? TotalHours { get; set; }
        
        /// <summary>
        /// Tipo de registro: CheckIn, CheckOut, Manual
        /// </summary>
        public string RegisterType { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha de entrada (solo fecha, sin hora)
        /// </summary>
        public DateOnly CheckInDateOnly { get; set; }
    }
}
