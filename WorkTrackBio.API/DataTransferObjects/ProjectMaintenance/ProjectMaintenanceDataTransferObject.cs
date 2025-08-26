namespace WorkTrackBio.API.DataTransferObjects.ProjectMaintenance
{
    public class ProjectMaintenanceDataTransferObject
    {
        public int Id { get; set; }
        public int IdProject { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string MaintenanceDescription { get; set; } = string.Empty;
        public int MadeById { get; set; }
        public decimal? MaintenanceCost { get; set; }
        public string? AdditionalInfo { get; set; }
        public int StateId { get; set; }
        
        // Navigation properties
        public string? ProjectName { get; set; }
        public string? MadeByName { get; set; }
        public string? StateName { get; set; }
    }
}
