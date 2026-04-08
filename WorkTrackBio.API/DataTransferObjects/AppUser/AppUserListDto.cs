namespace WorkTrackBio.API.DataTransferObjects.AppUser
{
    public class AppUserListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}