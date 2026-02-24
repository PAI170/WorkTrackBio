namespace WorkTrackBio.API.DataTransferObjects.ProjectWarranty
{
    public class ProjectWarrantyDataTransferObject
    {
        public int Id { get; set; }
        public int IdProject { get; set; }
        public DateTime WarrantyDate { get; set; }
        public string WarrantyDescription { get; set; } = string.Empty;
        public int MadeById { get; set; }
        public decimal? WarrantyCost { get; set; }
        public int StateId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string MadeByName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
    }
}
