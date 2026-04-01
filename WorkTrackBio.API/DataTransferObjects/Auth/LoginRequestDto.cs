namespace WorkTrackBio.API.DataTransferObjects.Auth
{
    public class LoginRequestDto
    {
        public string WorkEmail { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}