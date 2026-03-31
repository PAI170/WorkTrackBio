namespace WorkTrackBio.API.DataTransferObjects.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public UserInfoDto User { get; set; } = null!;
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}