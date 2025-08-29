namespace WorkTrackBio.API.DataTransferObjects.Assistance
{
    /// <summary>
    /// DTO para actualizar un registro de asistencia
    /// 
    /// IMPORTANTE: Para actualizaciones parciales:
    /// - Si un campo es NULL, NO se modifica (mantiene el valor actual)
    /// - Si un campo es string vacío "", se establece como NULL en la base de datos
    /// - Solo los campos enviados en el JSON serán actualizados
    /// </summary>
    public class UpdateAssistanceDataTransferObject
    {
        /// <summary>
        /// ID del empleado que registra la asistencia
        /// </summary>
        public int? EmployeeId { get; set; }
        
        /// <summary>
        /// ID del proyecto en el que trabaja
        /// </summary>
        public int? ProjectId { get; set; }
        
        /// <summary>
        /// Notas adicionales del registro
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Fecha y hora de entrada
        /// </summary>
        public DateTime? CheckIn { get; set; }
        
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
        public string? RegisterType { get; set; }
    }
}
