namespace WorkTrackBio.API.DataTransferObjects.Assistance
{
    /// <summary>
    /// DTO para crear un nuevo registro de asistencia
    /// </summary>
    public class CreateAssistanceDataTransferObject
    {
        /// <summary>
        /// ID del empleado que registra la asistencia
        /// </summary>
        public int EmployeeId { get; set; }
        
        /// <summary>
        /// ID del proyecto en el que trabaja
        /// </summary>
        public int ProjectId { get; set; }
        
        /// <summary>
        /// Notas adicionales del registro
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Fecha y hora de entrada
        /// </summary>
        public DateTime CheckIn { get; set; }
        
        /// <summary>
        /// Fecha y hora de salida (opcional para CheckIn)
        /// </summary>
        public DateTime? CheckOut { get; set; }
        
        /// <summary>
        /// Total de horas trabajadas (opcional, se puede calcular automáticamente)
        /// </summary>
        public decimal? TotalHours { get; set; }
        
        /// <summary>
        /// Tipo de registro: CheckIn, CheckOut, Manual
        /// </summary>
        public string RegisterType { get; set; } = string.Empty;
    }
}
