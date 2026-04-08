namespace WorkTrackBio.API.DataTransferObjects.Audit
{
    public class AuditLogResponseDto
    {
        public int Id { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int? RecordId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; }
    }
}