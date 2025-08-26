namespace WorkTrackBio.API.Validators.PhoneNumberFormatter
{
    /// <summary>
    /// Implementación del formateador de números de teléfono
    /// Utilidades para formatear números de teléfono según el patrón requerido por la base de datos
    /// </summary>
    public class PhoneNumberFormatter : IPhoneNumberFormatter
    {
        /// <summary>
        /// Formatea un número de teléfono al patrón requerido: nnnn-nnnn (4 dígitos, guión, 4 dígitos)
        /// </summary>
        /// <param name="phoneNumber">Número de teléfono a formatear (puede contener guiones, espacios, etc.)</param>
        /// <returns>Número formateado o null si el input es null o vacío</returns>
        public string? FormatPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            // Remover todos los caracteres no numéricos
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Si no hay exactamente 8 dígitos, retornar el original
            if (digitsOnly.Length != 8)
                return phoneNumber;

            // Formatear como nnnn-nnnn
            return $"{digitsOnly.Substring(0, 4)}-{digitsOnly.Substring(4, 4)}";
        }

        /// <summary>
        /// Formatea un número de documento removiendo guiones y espacios
        /// </summary>
        /// <param name="documentNumber">Número de documento a limpiar</param>
        /// <returns>Número limpio o null si el input es null o vacío</returns>
        public string? CleanDocumentNumber(string? documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return null;

            // Remover guiones, espacios y otros caracteres no alfanuméricos
            return new string(documentNumber.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }
    }
}

