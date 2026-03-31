using WorkTrackBio.API.DataTransferObjects.Auth;

namespace WorkTrackBio.API.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }
}