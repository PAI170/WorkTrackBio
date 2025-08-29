namespace WorkTrackBio.API.DataTransferObjects.ProjectWarranty
{
    /// <summary>
    /// DTO para operaciones de lectura de garantías de proyectos
    /// </summary>
    public class ProjectWarrantyDataTransferObject
    {
        public int Id { get; set; }
        public int IdProject { get; set; }
        public DateTime WarrantyDate { get; set; }
        public string WarrantyDescription { get; set; } = string.Empty;
        public int MadeById { get; set; }
        public decimal? WarrantyCost { get; set; }
        public int StateId { get; set; }
        
        // Propiedades de navegación para mostrar nombres
        public string ProjectName { get; set; } = string.Empty;
        public string MadeByName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
    }
}
