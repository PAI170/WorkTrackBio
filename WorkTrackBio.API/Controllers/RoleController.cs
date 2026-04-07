using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Catalog;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IRoleService _roleService;

        // ─── Constructor ──────────────────────────────────────────────────────
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _roleService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<RoleResponseDto>>
                .SuccessResponse(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _roleService.GetByIdAsync(id);
            return Ok(ApiResponse<RoleResponseDto>
                .SuccessResponse(data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleCreateDto dto)
        {
            var data = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.Id },
                ApiResponse<RoleResponseDto>
                    .SuccessResponse(data, "Rol creado exitosamente.", 201));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoleUpdateDto dto)
        {
            var data = await _roleService.UpdateAsync(id, dto);
            return Ok(ApiResponse<RoleResponseDto>
                .SuccessResponse(data, "Rol actualizado exitosamente."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Rol eliminado exitosamente."));
        }
    }
}