namespace WTB.API.Models.Configuration
{
    /// <summary>
    /// Configuración del sistema
    /// </summary>
    public class SystemSettings
    {
        /// <summary>
        /// ID del usuario del sistema para operaciones automáticas
        /// </summary>
        public int SystemUserId { get; set; } = 1;

        /// <summary>
        /// Nombre de usuario del sistema
        /// </summary>
        public string SystemUsername { get; set; } = "system";

        /// <summary>
        /// Email del usuario del sistema
        /// </summary>
        public string SystemEmail { get; set; } = "system@wtb.com";
    }
}
