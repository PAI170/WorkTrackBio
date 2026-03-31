using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Auth;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login exitoso"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Token renovado"));
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken);
            return Ok(ApiResponse<object>.SuccessResponse("Sesión cerrada exitosamente"));
        }
    }
}