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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _departmentService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<DepartmentResponseDto>>
                .SuccessResponse(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _departmentService.GetByIdAsync(id);
            return Ok(ApiResponse<DepartmentResponseDto>
                .SuccessResponse(data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartmentCreateDto dto)
        {
            var data = await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.Id },
                ApiResponse<DepartmentResponseDto>
                    .SuccessResponse(data, "Departamento creado exitosamente.", 201));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpdateDto dto)
        {
            var data = await _departmentService.UpdateAsync(id, dto);
            return Ok(ApiResponse<DepartmentResponseDto>
                .SuccessResponse(data, "Departamento actualizado exitosamente."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Departamento eliminado exitosamente."));
        }
    }
}