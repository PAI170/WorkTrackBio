namespace WorkTrackBio.API.DataTransferObjects.DocumentType
{
    public class UpdateDocumentTypeDataTransferObject
    {
        public int Id { get; set; }
        public string? DocumentName { get; set; }
        public string? Description { get; set; }
    }
}
