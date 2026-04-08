namespace WorkTrackBio.API.DataTransferObjects.AppUser
{
    public class AppUserUpdateDto
    {
        public int RoleId { get; set; }
        public string WorkEmail { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}