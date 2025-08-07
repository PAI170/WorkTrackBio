namespace WTB.API.Models.Enums
{
    /// <summary>
    /// Enum para los tipos de documento según la base de datos
    /// Debe coincidir con los IDs en la tabla DocumentType
    /// </summary>
    public enum DocumentTypeEnum
    {
        /// <summary>
        /// Cédula de identidad costarricense (n-nnnn-nnnn)
        /// </summary>
        CedulaIdentidad = 1,

        /// <summary>
        /// Pasaporte internacional (A1234567, CR12345678)
        /// </summary>
        Pasaporte = 2,

        /// <summary>
        /// Documento de Identidad Migratoria para Extranjeros (DIM-nnnnnnnn)
        /// </summary>
        DIMEX = 3,

        /// <summary>
        /// Permiso de trabajo para extranjeros (PT-nnnnnn)
        /// </summary>
        PermisoTrabajo = 4
    }
}
