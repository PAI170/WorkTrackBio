using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;
using WorkTrackBio.API.Services.ProjectMaintenanceService;
using WorkTrackBio.API.Validators.ProjectMaintenanceValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectMaintenanceController : ControllerBase
    {
        private readonly IProjectMaintenanceService _projectMaintenanceService;
        private readonly IProjectMaintenanceValidator _projectMaintenanceValidator;

        public ProjectMaintenanceController(
            IProjectMaintenanceService projectMaintenanceService,
            IProjectMaintenanceValidator projectMaintenanceValidator)
        {
            _projectMaintenanceService = projectMaintenanceService ?? throw new ArgumentNullException(nameof(projectMaintenanceService));
            _projectMaintenanceValidator = projectMaintenanceValidator ?? throw new ArgumentNullException(nameof(projectMaintenanceValidator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetAllProjectMaintenances()
        {
            var response = await _projectMaintenanceService.GetAllProjectMaintenancesAsync();
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok (response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenanceById(int id)
        {
            var response = await _projectMaintenanceService.GetProjectMaintenanceByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("project/{projectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetProjectMaintenancesByProject(int projectId)
        {
            var response = await _projectMaintenanceService.GetProjectMaintenancesByProjectAsync(projectId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("employee/{employeeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetProjectMaintenancesByEmployee(int employeeId)
        {
            var response = await _projectMaintenanceService.GetProjectMaintenancesByEmployeeAsync(employeeId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("state/{stateId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetProjectMaintenancesByState(int stateId)
        {
            var response = await _projectMaintenanceService.GetProjectMaintenancesByStateAsync(stateId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectMaintenanceDataTransferObject>>> CreateProjectMaintenance(CreateProjectMaintenanceDataTransferObject createDto)
        {
            var response = await _projectMaintenanceService.CreateProjectMaintenanceAsync(createDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetProjectMaintenanceById), new { id = response.Data.Id }, response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectMaintenanceDataTransferObject>>> UpdateProjectMaintenance(UpdateProjectMaintenanceDataTransferObject updateDto)
        {
            var response = await _projectMaintenanceService.UpdateProjectMaintenanceAsync(updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProjectMaintenance(int id)
        {
            var response = await _projectMaintenanceService.DeleteProjectMaintenanceAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}
