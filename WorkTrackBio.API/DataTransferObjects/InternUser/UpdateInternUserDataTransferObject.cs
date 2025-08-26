namespace WorkTrackBio.API.DataTransferObjects.InternUser
{
    public class UpdateInternUserDataTransferObject
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? RolId { get; set; }
        public int? StateId { get; set; }
        
        // Campos de documento
        public string? DocumentNumber { get; set; }
        public int? DocumentTypeId { get; set; }
        public DateOnly? DocumentExpire { get; set; }
    }
}

