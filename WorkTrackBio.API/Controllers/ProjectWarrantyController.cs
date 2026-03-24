using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;
using WorkTrackBio.API.Services.ProjectWarrantyService;

namespace WorkTrackBio.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProjectWarrantyController : ControllerBase
    {
        private readonly IProjectWarrantyService _projectWarrantyService;

        public ProjectWarrantyController(IProjectWarrantyService projectWarrantyService)
        {
            _projectWarrantyService = projectWarrantyService ?? throw new ArgumentNullException(nameof(projectWarrantyService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetAllProjectWarranties()
        {
            var response = await _projectWarrantyService.GetAllProjectWarrantiesAsync();
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<ProjectWarrantyDataTransferObject>>> GetProjectWarrantyById(int id)
        {
            var response = await _projectWarrantyService.GetProjectWarrantyByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("project/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByProject(int projectId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByProjectAsync(projectId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("employee/{employeeId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByEmployee(int employeeId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByEmployeeAsync(employeeId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("state/{stateId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByState(int stateId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByStateAsync(stateId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("daterange")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByDateRangeAsync(startDate, endDate);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("project/{projectId}/state/{stateId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 400)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>>> GetProjectWarrantiesByProjectAndState(
            int projectId, 
            int stateId)
        {
            var response = await _projectWarrantyService.GetProjectWarrantiesByDateRangeAsync(projectId, stateId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 201)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<ProjectWarrantyDataTransferObject>>> CreateProjectWarranty(CreateProjectWarrantyDataTransferObject createDto)
        {
            var response = await _projectWarrantyService.CreateProjectWarrantyAsync(createDto);      
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetProjectWarrantyById), new { id = response.Data!.Id }, response);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 400)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 404)]
        [ProducesResponseType(typeof(ApiResponse<ProjectWarrantyDataTransferObject>), 500)]
        public async Task<ActionResult<ApiResponse<ProjectWarrantyDataTransferObject>>> UpdateProjectWarranty(int id, UpdateProjectWarrantyDataTransferObject updateDto)
        {
            var response = await _projectWarrantyService.UpdateProjectWarrantyAsync(updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProjectWarranty(int id)
        {
            var response = await _projectWarrantyService.DeleteProjectWarrantyAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}
