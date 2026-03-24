using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Project;
using WorkTrackBio.API.Services.ProjectService;
using WorkTrackBio.API.Validators.ProjectValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService, IProjectValidator projectValidator)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDataTransferObject>>>> GetAllProjects()
        {
            var response = await _projectService.GetAllProjectsAsync();
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> GetProjectById(int id)
        {
            var response = await _projectService.GetProjectByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("name/{projectName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> GetProjectByName(string projectName)
        {
            var response = await _projectService.GetProjectByNameAsync(projectName);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("state/{stateId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDataTransferObject>>>> GetProjectsByState(int stateId)
        {
            var response = await _projectService.GetProjectsByStateAsync(stateId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> CreateProject(CreateProjectDataTransferObject createDto)
        {
            var response = await _projectService.CreateProjectAsync(createDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetProjectById), new { id = response.Data.Id }, response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> UpdateProject(UpdateProjectDataTransferObject updateDto)
        {
            var response = await _projectService.UpdateProjectAsync(updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProject(int id)
        {
            var response = await _projectService.DeleteProjectAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}
