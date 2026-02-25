using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Role;
using WorkTrackBio.API.Services.RoleService;
using WorkTrackBio.API.Validators.RoleValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IRoleValidator _roleValidator;

        public RoleController(IRoleService roleService, IRoleValidator roleValidator)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _roleValidator = roleValidator ?? throw new ArgumentNullException(nameof(roleValidator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDataTransferObject>>>> GetAllRoles()
        {
            var response = await _roleService.GetAllRolesAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> GetRoleById(int id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("name/{roleName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> GetRoleByName(string roleName)
        {
            var response = await _roleService.GetRoleByNameAsync(roleName);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> RoleExists(int id)
        {
            var response = await _roleService.RoleExistsAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("name/{roleName}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> RoleNameExists(string roleName)
        {
            var response = await _roleService.RoleNameExistsAsync(roleName);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> CreateRole([FromBody] CreateRoleDataTransferObject createDto)
        {
            if (createDto == null)
                return BadRequest(ApiResponse<RoleDataTransferObject>.ErrorResponse("Los datos del rol no pueden estar vacíos", 400));

            var validation = await _roleValidator.ValidateCreateAsync(createDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<RoleDataTransferObject>.ErrorResponse("Datos de entrada inválidos", 400, errors));
            }

            var response = await _roleService.CreateRoleAsync(createDto);

            if (!response.Success)
            {
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetRoleById), new { id = response.Data!.Id }, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> UpdateRole(int id, [FromBody] UpdateRoleDataTransferObject updateDto)
        {
            if (updateDto == null)
                return BadRequest(ApiResponse<RoleDataTransferObject>.ErrorResponse("Los datos del rol no pueden estar vacíos", 400));

            if (id != updateDto.Id)
                return BadRequest(ApiResponse<RoleDataTransferObject>.ErrorResponse("El ID de la URL no coincide con el ID del rol", 400));

            var validation = await _roleValidator.ValidateUpdateAsync(updateDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<RoleDataTransferObject>.ErrorResponse("Datos de entrada inválidos", 400, errors));
            }

            var response = await _roleService.UpdateRoleAsync(updateDto);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(int id)
        {
            var response = await _roleService.DeleteRoleAsync(id);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}