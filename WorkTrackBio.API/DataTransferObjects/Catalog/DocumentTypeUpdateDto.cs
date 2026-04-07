namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class DocumentTypeUpdateDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}