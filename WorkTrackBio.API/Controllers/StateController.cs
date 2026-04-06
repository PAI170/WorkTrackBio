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
    public class StateController : ControllerBase
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IStateService _stateService;

        // ─── Constructor ──────────────────────────────────────────────────────
        public StateController(IStateService stateService)
        {
            _stateService = stateService;
        }

        // ─── GET api/state ────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _stateService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<StateResponseDto>>
                .SuccessResponse(data));
        }

        // ─── GET api/state/{id} ───────────────────────────────────────────────
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _stateService.GetByIdAsync(id);
            return Ok(ApiResponse<StateResponseDto>
                .SuccessResponse(data));
        }

        // ─── POST api/state ───────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StateCreateDto dto)
        {
            var data = await _stateService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.Id },
                ApiResponse<StateResponseDto>
                    .SuccessResponse(data, "Estado creado exitosamente.", 201));
        }

        // ─── PUT api/state/{id} ───────────────────────────────────────────────
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] StateUpdateDto dto)
        {
            var data = await _stateService.UpdateAsync(id, dto);
            return Ok(ApiResponse<StateResponseDto>
                .SuccessResponse(data, "Estado actualizado exitosamente."));
        }

        // ─── DELETE api/state/{id} ────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _stateService.DeleteAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Estado eliminado exitosamente."));
        }
    }
}