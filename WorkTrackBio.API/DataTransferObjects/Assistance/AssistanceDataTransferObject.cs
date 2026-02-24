namespace WorkTrackBio.API.DataTransferObjects.Assistance
{
    public class AssistanceDataTransferObject
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public decimal? TotalHours { get; set; }
        public string RegisterType { get; set; } = string.Empty;
        public DateOnly CheckInDateOnly { get; set; }
    }
}
