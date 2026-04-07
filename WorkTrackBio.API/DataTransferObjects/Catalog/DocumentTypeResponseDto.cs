namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class DocumentTypeResponseDto
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EmployeeCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}