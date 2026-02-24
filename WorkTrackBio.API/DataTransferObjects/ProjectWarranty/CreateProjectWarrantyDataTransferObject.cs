namespace WorkTrackBio.API.DataTransferObjects.ProjectWarranty
{
    public class CreateProjectWarrantyDataTransferObject
    {
        public int IdProject { get; set; }
        public DateTime WarrantyDate { get; set; }
        public string WarrantyDescription { get; set; } = string.Empty;
        public int MadeById { get; set; }
        public decimal? WarrantyCost { get; set; }
        public int StateId { get; set; }
    }
}
