namespace WorkTrackBio.API.DataTransferObjects.InternUser
{
    public class InternUserDataTransferObject
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RolId { get; set; }
        public int StateId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
