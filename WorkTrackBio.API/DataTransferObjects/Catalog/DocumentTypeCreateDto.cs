namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class DocumentTypeCreateDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}