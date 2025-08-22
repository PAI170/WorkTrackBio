namespace WorkTrackBio.API.DataTransferObjects.DocumentType
{
    public class CreateDocumentTypeDataTransferObject
    {
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
