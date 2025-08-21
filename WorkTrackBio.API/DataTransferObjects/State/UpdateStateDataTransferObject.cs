namespace WorkTrackBio.API.DataTransferObjects.State
{
    /// <summary>
    /// DTO para actualizar estados con soporte para "Partial Updates"
    /// </summary>
    public class UpdateStateDataTransferObject
    {
        /// <summary>
        /// ID del estado a actualizar (obligatorio)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nuevo nombre del estado
        /// - null = No cambiar (mantener valor actual)
        /// - "NuevoNombre" = Cambiar a este valor
        /// - "" = Cambiar a string vacío (NO RECOMENDADO)
        /// </summary>
        public string? StateName { get; set; }

        /// <summary>
        /// Nuevo tipo de estado
        /// - null = No cambiar (mantener valor actual)
        /// - "NuevoTipo" = Cambiar a este valor
        /// - "" = Cambiar a string vacío (NO RECOMENDADO)
        /// </summary>
        public string? StateType { get; set; }

        /// <summary>
        /// Nueva descripción del estado
        /// - null = No cambiar (mantener valor actual)
        /// - "Nueva descripción" = Cambiar a este valor
        /// - "" = Cambiar a string vacío (permitido)
        /// </summary>
        public string? Description { get; set; }
    }
}
