namespace WorkTrackBio.API.DataTransferObjects.AppUser
{
    public class AppUserCreateDto
    {
        public int EmployeeInfoId { get; set; }
        public int RoleId { get; set; }
        public string WorkEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}