namespace WorkTrackBio.API.Common
{
    /// <summary>
    /// Resultado de la validación de un documento
    /// </summary>
    public class DocumentValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

