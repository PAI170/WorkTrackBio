using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.AppUser;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AppUserController : ControllerBase
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IAppUserService _appUserService;

        // ─── Constructor ──────────────────────────────────────────────────────
        public AppUserController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _appUserService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<AppUserListDto>>
                .SuccessResponse(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _appUserService.GetByIdAsync(id);
            return Ok(ApiResponse<AppUserDetailDto>
                .SuccessResponse(data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppUserCreateDto dto)
        {
            var data = await _appUserService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.Id },
                ApiResponse<AppUserDetailDto>
                    .SuccessResponse(data, "Usuario creado exitosamente.", 201));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppUserUpdateDto dto)
        {
            var data = await _appUserService.UpdateAsync(id, dto);
            return Ok(ApiResponse<AppUserDetailDto>
                .SuccessResponse(data, "Usuario actualizado exitosamente."));
        }

        [HttpPut("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] AppUserResetPasswordDto dto)
        {
            await _appUserService.ResetPasswordAsync(id, dto);
            return Ok(ApiResponse<object>
                .SuccessResponse("Contraseña restablecida exitosamente."));
        }

        [HttpPut("{id:int}/unlock")]
        public async Task<IActionResult> Unlock(int id)
        {
            await _appUserService.UnlockAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Cuenta desbloqueada exitosamente."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _appUserService.DeleteAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Usuario eliminado exitosamente."));
        }
    }
}