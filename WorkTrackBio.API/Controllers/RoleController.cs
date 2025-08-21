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

        /// <summary>
        /// Obtiene todos los roles
        /// </summary>
        /// <returns>Lista de todos los roles</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<RoleDataTransferObject>>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                var response = ApiResponse<IEnumerable<RoleDataTransferObject>>.SuccessResponse(
                    roles, 
                    "Roles obtenidos exitosamente", 
                    StatusCodes.Status200OK);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<RoleDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Obtiene un rol por su ID
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Rol encontrado o NotFound</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> GetRoleById(int id)
        {
            try
            {
                // Validar ID
                var idValidation = _roleValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var role = await _roleService.GetRoleByIdAsync(id);
                
                if (role == null)
                {
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        $"No se encontró un rol con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<RoleDataTransferObject>.SuccessResponse(
                    role, 
                    "Rol obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Obtiene un rol por su nombre
        /// </summary>
        /// <param name="roleName">Nombre del rol</param>
        /// <returns>Rol encontrado o NotFound</returns>
        [HttpGet("name/{roleName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> GetRoleByName(string roleName)
        {
            try
            {
                // Validar nombre del rol
                var nameValidation = _roleValidator.ValidateRoleName(roleName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "Nombre de rol inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var role = await _roleService.GetRoleByNameAsync(roleName);
                
                if (role == null)
                {
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        $"No se encontró un rol con el nombre '{roleName}'", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<RoleDataTransferObject>.SuccessResponse(
                    role, 
                    "Rol obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Crea un nuevo rol
        /// </summary>
        /// <param name="createDto">Datos del rol a crear</param>
        /// <returns>Rol creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> CreateRole([FromBody] CreateRoleDataTransferObject createDto)
        {
            try
            {
                if (createDto == null)
                {
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "Los datos del rol no pueden estar vacíos", 
                        StatusCodes.Status400BadRequest);
                    return BadRequest(response);
                }

                // Validar datos de entrada
                var validation = await _roleValidator.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "Datos de entrada inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var createdRole = await _roleService.CreateRoleAsync(createDto);
                
                var successResponse = ApiResponse<RoleDataTransferObject>.SuccessResponse(
                    createdRole, 
                    "Rol creado exitosamente", 
                    StatusCodes.Status201Created);
                
                return CreatedAtAction(
                    nameof(GetRoleById), 
                    new { id = createdRole.Id }, 
                    successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Actualiza un rol existente
        /// </summary>
        /// <param name="id">ID del rol a actualizar</param>
        /// <param name="updateDto">Datos actualizados del rol</param>
        /// <returns>Rol actualizado o NotFound</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<RoleDataTransferObject>>> UpdateRole(int id, [FromBody] UpdateRoleDataTransferObject updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "Los datos del rol no pueden estar vacíos", 
                        StatusCodes.Status400BadRequest);
                    return BadRequest(response);
                }

                if (id != updateDto.Id)
                {
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "El ID de la URL no coincide con el ID del rol", 
                        StatusCodes.Status400BadRequest);
                    return BadRequest(response);
                }

                // Validar datos de entrada
                var validation = await _roleValidator.ValidateUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        "Datos de entrada inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var updatedRole = await _roleService.UpdateRoleAsync(updateDto);
                
                if (updatedRole == null)
                {
                    var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                        $"No se encontró un rol con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<RoleDataTransferObject>.SuccessResponse(
                    updatedRole, 
                    "Rol actualizado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<RoleDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Elimina un rol
        /// </summary>
        /// <param name="id">ID del rol a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRole(int id)
        {
            try
            {
                // Validar ID
                var idValidation = _roleValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<object>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var deleted = await _roleService.DeleteRoleAsync(id);
                
                if (!deleted)
                {
                    var response = ApiResponse<object>.ErrorResponse(
                        $"No se encontró un rol con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<object>.SuccessResponse(
                    "Rol eliminado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<object>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Verifica si un rol existe
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>True si existe, False si no</returns>
        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> RoleExists(int id)
        {
            try
            {
                // Validar ID
                var idValidation = _roleValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _roleService.RoleExistsAsync(id);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "El rol existe" : "El rol no existe", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Verifica si existe un rol con un nombre específico
        /// </summary>
        /// <param name="roleName">Nombre del rol</param>
        /// <returns>True si existe, False si no</returns>
        [HttpGet("name/{roleName}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> RoleNameExists(string roleName)
        {
            try
            {
                // Validar nombre del rol
                var nameValidation = _roleValidator.ValidateRoleName(roleName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "Nombre de rol inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _roleService.RoleNameExistsAsync(roleName);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "Ya existe un rol con ese nombre" : "No existe un rol con ese nombre", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
