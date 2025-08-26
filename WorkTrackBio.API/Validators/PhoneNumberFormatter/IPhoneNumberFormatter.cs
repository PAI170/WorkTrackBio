namespace WorkTrackBio.API.Validators.PhoneNumberFormatter
{
    /// <summary>
    /// Interfaz para el formateador de números de teléfono
    /// </summary>
    public interface IPhoneNumberFormatter
    {
        /// <summary>
        /// Formatea un número de teléfono al patrón requerido: nnnn-nnnn (4 dígitos, guión, 4 dígitos)
        /// </summary>
        /// <param name="phoneNumber">Número de teléfono a formatear (puede contener guiones, espacios, etc.)</param>
        /// <returns>Número formateado o null si el input es null o vacío</returns>
        string? FormatPhoneNumber(string? phoneNumber);

        /// <summary>
        /// Formatea un número de documento removiendo guiones y espacios
        /// </summary>
        /// <param name="documentNumber">Número de documento a limpiar</param>
        /// <returns>Número limpio o null si el input es null o vacío</returns>
        string? CleanDocumentNumber(string? documentNumber);
    }
}

