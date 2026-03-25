using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.InternUser;
using WorkTrackBio.API.Services.InternUserService;
using WorkTrackBio.API.Validators.InternUserValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InternUserController : ControllerBase
    {
        private readonly IInternUserService _internUserService;

        public InternUserController(
            IInternUserService internUserService)
        {
            _internUserService = internUserService ?? throw new ArgumentNullException(nameof(internUserService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetAllInternUsers()
        {
            var response = await _internUserService.GetAllInternUsersAsync();
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> GetInternUserById(int id)
        {
            var response = await _internUserService.GetInternUserByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> GetInternUserByEmail(string email)
        {
            var response = await _internUserService.GetInternUserByEmailAsync(email);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("role/{roleId:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetInternUsersByRole(int roleId)
        {
            var response = await _internUserService.GetInternUsersByRoleAsync(roleId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpGet("state/{stateId:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetInternUsersByState(int stateId)
        {
            var response = await _internUserService.GetInternUsersByStateAsync(stateId);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> CreateInternUser([FromBody] CreateInternUserDataTransferObject createDto)
        {
            var response = await _internUserService.CreateInternUserAsync(createDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetInternUserById), new { id = response.Data!.Id }, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> UpdateInternUser(int id, [FromBody] UpdateInternUserDataTransferObject updateDto)
        {
            var response = await _internUserService.UpdateInternUserAsync(updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteInternUser(int id)
        {
            var response = await _internUserService.DeleteInternUserAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}
