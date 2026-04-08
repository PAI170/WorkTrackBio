using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Employee;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IEmployeeService _employeeService;

        // ─── Constructor ──────────────────────────────────────────────────────
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _employeeService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<EmployeeListDto>>
                .SuccessResponse(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _employeeService.GetByIdAsync(id);
            return Ok(ApiResponse<EmployeeDetailDto>
                .SuccessResponse(data));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
        {
            var data = await _employeeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.Id },
                ApiResponse<EmployeeDetailDto>
                    .SuccessResponse(data, "Empleado creado exitosamente.", 201));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var data = await _employeeService.UpdateAsync(id, dto);
            return Ok(ApiResponse<EmployeeDetailDto>
                .SuccessResponse(data, "Empleado actualizado exitosamente."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Empleado eliminado exitosamente."));
        }
    }
}