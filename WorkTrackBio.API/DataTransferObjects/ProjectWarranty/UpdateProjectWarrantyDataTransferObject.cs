namespace WorkTrackBio.API.DataTransferObjects.ProjectWarranty
{
    /// <summary>
    /// DTO para actualizaciones parciales de garantías de proyectos
    /// </summary>
    public class UpdateProjectWarrantyDataTransferObject
    {
        public int? IdProject { get; set; }
        public DateTime? WarrantyDate { get; set; }
        public string? WarrantyDescription { get; set; }
        public int? MadeById { get; set; }
        public decimal? WarrantyCost { get; set; }
        public int? StateId { get; set; }
    }
}
