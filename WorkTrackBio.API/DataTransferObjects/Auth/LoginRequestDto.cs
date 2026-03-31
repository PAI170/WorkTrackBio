using System.ComponentModel.DataAnnotations;

namespace WorkTrackBio.API.DataTransferObjects.Auth
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string WorkEmail { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}