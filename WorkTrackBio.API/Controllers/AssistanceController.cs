using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Assistance;
using WorkTrackBio.API.Services.AssistanceService;
using WorkTrackBio.API.Validators.AssistanceValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AssistanceController : ControllerBase
    {
        private readonly IAssistanceService _assistanceService;

        public AssistanceController(
            IAssistanceService assistanceService)
        {
            _assistanceService = assistanceService ?? throw new ArgumentNullException(nameof(assistanceService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAllAssistances()
        {
            var response = await _assistanceService.GetAllAssistancesAsync();
            if(!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<AssistanceDataTransferObject>>> GetAssistanceById(int id)
        {
            var response = await _assistanceService.GetAssistanceByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByEmployee(int employeeId)
        {
            var response = await _assistanceService.GetAssistancesByEmployeeAsync(employeeId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByProject(int projectId)
        {
            var response = await _assistanceService.GetAssistancesByProjectAsync(projectId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("date/{date:datetime}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByDate(DateTime date)
        {
            var response = await _assistanceService.GetAssistancesByDateAsync(date);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var response = await _assistanceService.GetAssistancesByDateRangeAsync(startDateOnly, endDateOnly);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("employee/{employeeId}/project/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByEmployeeAndProject(int employeeId, int projectId)
        {
            var response = await _assistanceService.GetAssistancesByEmployeeAndProjectAsync(employeeId, projectId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("employee/{employeeId}/daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByEmployeeAndDateRange(
            int employeeId,
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var response = await _assistanceService.GetAssistancesByEmployeeAndDateRangeAsync(employeeId, startDateOnly, endDateOnly);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("project/{projectId}/daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByProjectAndDateRange(
            int projectId,
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            var response = await _assistanceService.GetAssistancesByProjectAndDateRangeAsync(projectId, startDateOnly, endDateOnly);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("project/{projectId}/employee/{employeeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssistanceDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssistanceDataTransferObject>>>> GetAssistancesByProjectAndEmployee(
            int projectId, 
            int employeeId)
        {
            var response = await _assistanceService.GetAssistancesByProjectAsync(employeeId, projectId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 201)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<AssistanceDataTransferObject>>> CreateAssistance(CreateAssistanceDataTransferObject createDto)
        {
            var response = await _assistanceService.CreateAssistanceAsync(createDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetAssistanceById), new { id = response.Data!.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<AssistanceDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<AssistanceDataTransferObject>>> UpdateAssistance(int id, UpdateAssistanceDataTransferObject updateDto)
        {
            var response = await _assistanceService.UpdateAssistanceAsync(int id, updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteAssistance(int id)
        {
            var response = await _assistanceService.DeleteAssistanceAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("{id}/calculate-hours")]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 200)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 400)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 404)]
        [ProducesResponseType(typeof(ApiResponse<decimal>), 500)]
        public async Task<ActionResult<ApiResponse<decimal>>> CalculateTotalHours(int id)
        {
            var response = await _assistanceService.CalculateTotalHoursAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}
