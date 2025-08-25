namespace WorkTrackBio.API.DataTransferObjects.InternUser
{
    public class CreateInternUserDataTransferObject
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RolId { get; set; }
        public int StateId { get; set; }
    }
}
