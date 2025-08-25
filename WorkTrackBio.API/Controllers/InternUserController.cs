using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.InternUser;
using WorkTrackBio.API.Services.InternUserService;
using WorkTrackBio.API.Validators.InternUserValidator;

namespace WorkTrackBio.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar usuarios internos del sistema
    /// </summary>
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

        /// <summary>
        /// Obtiene todos los usuarios internos
        /// </summary>
        /// <returns>Lista de usuarios internos</returns>
        /// <response code="200">Lista de usuarios obtenida exitosamente</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InternUserDataTransferObject>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InternUserDataTransferObject>>>> GetAllInternUsers()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los usuarios internos");
                
                var internUsers = await _internUserService.GetAllInternUsersAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} usuarios internos", internUsers.Count());
                
                return Ok(ApiResponse<IEnumerable<InternUserDataTransferObject>>.SuccessResponse(
                    internUsers, 
                    "Usuarios internos obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios internos");
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener usuarios internos"));
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por su ID
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>Usuario interno encontrado</returns>
        /// <response code="200">Usuario interno obtenido exitosamente</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Usuario interno no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó obtener usuario interno con ID inválido: {Id}", id);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID debe ser mayor que 0"));
                }

                _logger.LogInformation("Obteniendo usuario interno con ID: {Id}", id);
                
                var internUser = await _internUserService.GetInternUserByIdAsync(id);
                
                if (internUser == null)
                {
                    _logger.LogWarning("No se encontró usuario interno con ID: {Id}", id);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario interno con ID {id}"));
                }
                
                _logger.LogInformation("Usuario interno obtenido exitosamente con ID: {Id}", id);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUser, 
                    "Usuario interno obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario interno con ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener el usuario interno"));
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por su email
        /// </summary>
        /// <param name="email">Email del usuario interno</param>
        /// <returns>Usuario interno encontrado</returns>
        /// <response code="200">Usuario interno obtenido exitosamente</response>
        /// <response code="400">Email inválido</response>
        /// <response code="404">Usuario interno no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó obtener usuario interno con email vacío");
                    return BadRequest(ApiResponse<object>.ErrorResponse("El email no puede estar vacío"));
                }

                _logger.LogInformation("Obteniendo usuario interno con email: {Email}", email);
                
                var internUser = await _internUserService.GetInternUserByEmailAsync(email);
                
                if (internUser == null)
                {
                    _logger.LogWarning("No se encontró usuario interno con email: {Email}", email);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario interno con email '{email}'"));
                }
                
                _logger.LogInformation("Usuario interno obtenido exitosamente con email: {Email}", email);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUser, 
                    "Usuario interno obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario interno con email: {Email}", email);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener el usuario interno"));
            }
        }

        /// <summary>
        /// Obtiene usuarios internos por rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de usuarios internos del rol especificado</returns>
        /// <response code="200">Usuarios internos obtenidos exitosamente</response>
        /// <response code="400">ID de rol inválido</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó obtener usuarios internos con ID de rol inválido: {RoleId}", roleId);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID del rol debe ser mayor que 0"));
                }

                _logger.LogInformation("Obteniendo usuarios internos del rol con ID: {RoleId}", roleId);
                
                var internUsers = await _internUserService.GetInternUsersByRoleAsync(roleId);
                
                _logger.LogInformation("Se obtuvieron {Count} usuarios internos del rol con ID: {RoleId}", 
                    internUsers.Count(), roleId);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUsers, 
                    $"Usuarios internos del rol {roleId} obtenidos exitosamente"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al obtener usuarios internos por rol: {RoleId}", roleId);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios internos del rol con ID: {RoleId}", roleId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener usuarios internos por rol"));
            }
        }

        /// <summary>
        /// Obtiene usuarios internos por estado
        /// </summary>
        /// <param name="stateId">ID del estado</param>
        /// <returns>Lista de usuarios internos del estado especificado</returns>
        /// <response code="200">Usuarios internos obtenidos exitosamente</response>
        /// <response code="400">ID de estado inválido</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó obtener usuarios internos con ID de estado inválido: {StateId}", stateId);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID del estado debe ser mayor que 0"));
                }

                _logger.LogInformation("Obteniendo usuarios internos del estado con ID: {StateId}", stateId);
                
                var internUsers = await _internUserService.GetInternUsersByStateAsync(stateId);
                
                _logger.LogInformation("Se obtuvieron {Count} usuarios internos del estado con ID: {StateId}", 
                    internUsers.Count(), stateId);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    internUsers, 
                    $"Usuarios internos del estado {stateId} obtenidos exitosamente"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al obtener usuarios internos por estado: {StateId}", stateId);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios internos del estado con ID: {StateId}", stateId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al obtener usuarios internos por estado"));
            }
        }

        /// <summary>
        /// Crea un nuevo usuario interno
        /// </summary>
        /// <param name="createDto">Datos para crear el usuario interno</param>
        /// <returns>Usuario interno creado</returns>
        /// <response code="201">Usuario interno creado exitosamente</response>
        /// <response code="400">Datos inválidos o validación fallida</response>
        /// <response code="409">Conflicto (email ya existe)</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó crear usuario interno con DTO nulo");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Los datos del usuario interno no pueden ser nulos"));
                }

                _logger.LogInformation("Creando nuevo usuario interno con email: {Email}", createDto.Email);
                
                // Validar DTO antes de crear
                var validationResult = await _internUserValidator.ValidateCreateAsync(createDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validación fallida al crear usuario interno: {Errors}", errors);
                    return BadRequest(ApiResponse<object>.ErrorResponse($"Validación fallida: {errors}"));
                }
                
                var createdInternUser = await _internUserService.CreateInternUserAsync(createDto);
                
                _logger.LogInformation("Usuario interno creado exitosamente con ID: {Id}", createdInternUser.Id);
                
                return CreatedAtAction(
                    nameof(GetInternUserById), 
                    new { id = createdInternUser.Id },
                    ApiResponse<object>.SuccessResponse(
                        createdInternUser, 
                        "Usuario interno creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflicto al crear usuario interno con email: {Email}", createDto?.Email);
                return Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al crear usuario interno");
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario interno con email: {Email}", createDto?.Email);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al crear el usuario interno"));
            }
        }

        /// <summary>
        /// Actualiza un usuario interno existente
        /// </summary>
        /// <param name="id">ID del usuario interno a actualizar</param>
        /// <param name="updateDto">Datos para actualizar el usuario interno</param>
        /// <returns>Usuario interno actualizado</returns>
        /// <response code="200">Usuario interno actualizado exitosamente</response>
        /// <response code="400">Datos inválidos o validación fallida</response>
        /// <response code="404">Usuario interno no encontrado</response>
        /// <response code="409">Conflicto (email ya existe)</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó actualizar usuario interno con DTO nulo");
                    return BadRequest(ApiResponse<object>.ErrorResponse("Los datos de actualización no pueden ser nulos"));
                }

                if (id != updateDto.Id)
                {
                    _logger.LogWarning("ID en ruta ({RouteId}) no coincide con ID en DTO ({DtoId})", id, updateDto.Id);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID en la ruta debe coincidir con el ID en el cuerpo de la petición"));
                }

                _logger.LogInformation("Actualizando usuario interno con ID: {Id}", id);
                
                // Validar DTO antes de actualizar
                var validationResult = await _internUserValidator.ValidateUpdateAsync(updateDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Validación fallida al actualizar usuario interno con ID: {Id}. Errores: {Errors}", id, errors);
                    return BadRequest(ApiResponse<object>.ErrorResponse($"Validación fallida: {errors}"));
                }
                
                var updatedInternUser = await _internUserService.UpdateInternUserAsync(updateDto);
                
                if (updatedInternUser == null)
                {
                    _logger.LogWarning("No se encontró usuario interno para actualizar con ID: {Id}", id);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario interno con ID {id}"));
                }
                
                _logger.LogInformation("Usuario interno actualizado exitosamente con ID: {Id}", id);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    updatedInternUser, 
                    "Usuario interno actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflicto al actualizar usuario interno con ID: {Id}", id);
                return Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argumento inválido al actualizar usuario interno con ID: {Id}", id);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario interno con ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al actualizar el usuario interno"));
            }
        }

        /// <summary>
        /// Elimina un usuario interno
        /// </summary>
        /// <param name="id">ID del usuario interno a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Usuario interno eliminado exitosamente</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Usuario interno no encontrado</response>
        /// <response code="409">No se puede eliminar (tiene dependencias)</response>
        /// <response code="500">Error interno del servidor</response>
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
                    _logger.LogWarning("Se intentó eliminar usuario interno con ID inválido: {Id}", id);
                    return BadRequest(ApiResponse<object>.ErrorResponse("El ID debe ser mayor que 0"));
                }

                _logger.LogInformation("Eliminando usuario interno con ID: {Id}", id);
                
                var deleted = await _internUserService.DeleteInternUserAsync(id);
                
                if (!deleted)
                {
                    _logger.LogWarning("No se encontró usuario interno para eliminar con ID: {Id}", id);
                    return NotFound(ApiResponse<object>.ErrorResponse($"No se encontró un usuario interno con ID {id}"));
                }
                
                _logger.LogInformation("Usuario interno eliminado exitosamente con ID: {Id}", id);
                
                return Ok(ApiResponse<object>.SuccessResponse(
                    "Usuario interno eliminado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No se puede eliminar usuario interno con ID: {Id} - Tiene dependencias", id);
                return Conflict(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario interno con ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse(
                    "Error interno del servidor al eliminar el usuario interno"));
            }
        }
    }
}
