namespace WorkTrackBio.API.DataTransferObjects.Assistance
{
    public class UpdateAssistanceDataTransferObject
    {
        public int? EmployeeId { get; set; }
        public int? ProjectId { get; set; }
        public string? Notes { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public decimal? TotalHours { get; set; }
        public string? RegisterType { get; set; }
    }
}
