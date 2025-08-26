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
        private readonly IInternUserValidator _internUserValidator;
        private readonly ILogger<InternUserController> _logger;

        public InternUserController(
            IInternUserService internUserService,
            IInternUserValidator internUserValidator,
            ILogger<InternUserController> logger)
        {
            _internUserService = internUserService ?? throw new ArgumentNullException(nameof(internUserService));
            _internUserValidator = internUserValidator ?? throw new ArgumentNullException(nameof(internUserValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetAllInternUsers()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los usuarios");
                
                var internUsers = await _internUserService.GetAllInternUsersAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} usuarios", internUsers.Count());
                
                return Ok(ApiResponse<IEnumerable<InternUserDataTransferObject>>.SuccessResponse(
                    internUsers, 
                    "Usuarios obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios internos");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener usuarios internos"));
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> GetInternUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Se intentó obtener usuario con ID inválido: {Id}", id);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID debe ser mayor que 0"));
                }

                _logger.LogInformation("Obteniendo usuario con ID: {Id}", id);
                
                var internUser = await _internUserService.GetInternUserByIdAsync(id);
                
                if (internUser == null)
                {
                    _logger.LogWarning("No se encontró usuario interno con ID: {Id}", id);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario con ID {id}"));
                }
                
                _logger.LogInformation("Usuario obtenido exitosamente con ID: {Id}", id);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUser, 
                    "Usuario obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener el usuario interno"));
            }
        }

        [HttpGet("email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> GetInternUserByEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Se intentó obtener usuario con email vacío");
                    return BadRequest(ApiResponse<object>.ErrorResponse("El email no puede estar vacío"));
                }

                _logger.LogInformation("Obteniendo usuario con email: {Email}", email);
                
                var internUser = await _internUserService.GetInternUserByEmailAsync(email);
                
                if (internUser == null)
                {
                    _logger.LogWarning("No se encontró usuario con email: {Email}", email);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario con email '{email}'"));
                }
                
                _logger.LogInformation("Usuario obtenido exitosamente con email: {Email}", email);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUser, 
                    "Usuario obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con email: {Email}", email);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener el usuario"));
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por ID o número de documento
        /// </summary>
        /// <param name="identifier">ID del usuario o número de documento</param>
        /// <returns>Usuario interno encontrado</returns>
        /// <response code="200">Usuario interno obtenido exitosamente</response>
        /// <response code="400">Identificador inválido</response>
        /// <response code="404">Usuario interno no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("search/{identifier}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> GetInternUserByIdentifier(string identifier)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    _logger.LogWarning("Se intentó obtener usuario con identificador vacío");
                    return BadRequest(ApiResponse<object>.ErrorResponse("El identificador no puede estar vacío"));
                }

                _logger.LogInformation("Buscando usuario con identificador: {Identifier}", identifier);
                
                var internUser = await _internUserService.GetInternUserByIdentifierAsync(identifier);
                
                if (internUser == null)
                {
                    _logger.LogWarning("No se encontró usuario con identificador: {Identifier}", identifier);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario con el identificador '{identifier}'"));
                }
                
                _logger.LogInformation("Usuario obtenido exitosamente con identificador: {Identifier}", identifier);
                
                return Ok(ApiResponse<InternUserDataTransferObject>.SuccessResponse(
                    internUser, 
                    "Usuario obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con identificador: {Identifier}", identifier);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener el usuario"));
            }
        }

        [HttpGet("role/{roleId:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetInternUsersByRole(int roleId)
        {
            try
            {
                if (roleId <= 0)
                {
                    _logger.LogWarning("Se intentó obtener usuarios con ID de rol inválido: {RoleId}", roleId);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID del rol debe ser mayor que 0"));
                }

                _logger.LogInformation("Obteniendo usuarios del rol con ID: {RoleId}", roleId);
                
                var internUsers = await _internUserService.GetInternUsersByRoleAsync(roleId);
                
                _logger.LogInformation("Se obtuvieron {Count} usuarios del rol con ID: {RoleId}", 
                    internUsers.Count(), roleId);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUsers, 
                    $"Usuarios del rol {roleId} obtenidos exitosamente"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al obtener usuarios por rol: {RoleId}", roleId);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios del rol con ID: {RoleId}", roleId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener usuarios por rol"));
            }
        }

        [HttpGet("state/{stateId:int}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetInternUsersByState(int stateId)
        {
            try
            {
                if (stateId <= 0)
                {
                    _logger.LogWarning("Se intentó obtener usuarios con ID de estado inválido: {StateId}", stateId);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID del estado debe ser mayor que 0"));
                }

                _logger.LogInformation("Obteniendo usuarios del estado con ID: {StateId}", stateId);
                
                var internUsers = await _internUserService.GetInternUsersByStateAsync(stateId);
                
                _logger.LogInformation("Se obtuvieron {Count} usuarios del estado con ID: {StateId}", 
                    internUsers.Count(), stateId);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUsers, 
                    $"Usuarios del estado {stateId} obtenidos exitosamente"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al obtener usuarios por estado: {StateId}", stateId);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios del estado con ID: {StateId}", stateId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener usuarios por estado"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> CreateInternUser([FromBody] CreateInternUserDataTransferObject createDto)
        {
            try
            {
                if (createDto == null)
                {
                    _logger.LogWarning("Se intentó crear usuario con DTO nulo");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Los datos del usuario no pueden estar vacios"));
                }

                _logger.LogInformation("Creando nuevo usuario con email: {Email}", createDto.Email);
                
                var validationResult = await _internUserValidator.ValidateCreateAsync(createDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validación fallida al crear usuario: {Errors}", errors);
                    return BadRequest(ApiResponse<object>.ErrorResponse($"Validación fallida: {errors}"));
                }
                
                var createdInternUser = await _internUserService.CreateInternUserAsync(createDto);
                
                _logger.LogInformation("Usuario creado exitosamente con ID: {Id}", createdInternUser.Id);
                
                return CreatedAtAction(
                    nameof(GetInternUserById), 
                    new { id = createdInternUser.Id },
                    ApiResponse<object>.SuccessResponse(
                        createdInternUser, 
                        "Usuario creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflicto al crear usuario con email: {Email}", createDto?.Email);
                return Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al crear usuario");
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario con email: {Email}", createDto?.Email);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al crear el usuario"));
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<InternUserDataTransferObject>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<InternUserDataTransferObject>>> UpdateInternUser(int id, [FromBody] UpdateInternUserDataTransferObject updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    _logger.LogWarning("Se intentó actualizar usuario con DTO nulo");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Campos necesarios para la actualizacion"));
                }

                if (id != updateDto.Id)
                {
                    _logger.LogWarning("ID en ruta ({RouteId}) no coincide con ID en DTO ({DtoId})", id, updateDto.Id);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID en la ruta debe coincidir con el ID en el cuerpo de la petición"));
                }

                _logger.LogInformation("Actualizando usuario con ID: {Id}", id);
                
                // Validar DTO antes de actualizar
                var validationResult = await _internUserValidator.ValidateUpdateAsync(updateDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validación fallida al actualizar usuario con ID: {Id}. Errores: {Errors}", id, errors);
                    return BadRequest(ApiResponse<object>.ErrorResponse($"Validación fallida: {errors}"));
                }
                
                var updatedInternUser = await _internUserService.UpdateInternUserAsync(updateDto);
                
                if (updatedInternUser == null)
                {
                    _logger.LogWarning("No se encontró usuario para actualizar con ID: {Id}", id);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario con ID {id}"));
                }
                
                _logger.LogInformation("Usuario actualizado exitosamente con ID: {Id}", id);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    updatedInternUser, 
                    "Usuario actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflicto al actualizar usuario con ID: {Id}", id);
                return Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al actualizar usuario con ID: {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario con ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al actualizar el usuario"));
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteInternUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Se intentó eliminar usuario con ID inválido: {Id}", id);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID debe ser mayor que 0"));
                }

                _logger.LogInformation("Eliminando usuario con ID: {Id}", id);
                
                var deleted = await _internUserService.DeleteInternUserAsync(id);
                
                if (!deleted)
                {
                    _logger.LogWarning("No se encontró usuario para eliminar con ID: {Id}", id);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario con ID {id}"));
                }
                
                _logger.LogInformation("Usuario eliminado exitosamente con ID: {Id}", id);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    "Usuario eliminado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No se puede eliminar usuario con ID: {Id} - Tiene dependencias", id);
                return Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al eliminar el usuario"));
            }
        }
    }
}
