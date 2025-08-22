namespace WorkTrackBio.API.DataTransferObjects.DocumentType
{
    public class DocumentTypeDataTransferObject
    {
        public int Id { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
