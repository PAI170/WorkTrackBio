using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.InternUser;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de usuarios internos del sistema
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InternUsersController : ControllerBase
    {
        private readonly IInternUserService _internUserService;
        private readonly ILogger<InternUsersController> _logger;

        public InternUsersController(
            IInternUserService internUserService,
            ILogger<InternUsersController> logger)
        {
            _internUserService = internUserService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene lista paginada de usuarios internos con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de usuarios internos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<PagedResponse<InternUserResponseDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInternUsers([FromQuery] InternUserFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos con filtros");

                var result = await _internUserService.GetFilteredAsync(filter);

                _logger.LogInformation("Usuarios internos obtenidos: {Count} de {Total}", 
                    result.Data?.Count() ?? 0, result.TotalRecords);

                return Ok(APIResponse<PagedResponse<InternUserResponseDto>>.SuccessResponse(result, 
                    "Usuarios internos obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por ID
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>Usuario interno si existe</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<InternUserDetailsDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInternUser(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuario interno con ID: {Id}", id);

                var user = await _internUserService.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario interno con ID {Id} no encontrado", id);
                    return NotFound(APIResponse.ErrorResponse($"Usuario interno con ID {id} no encontrado", HttpStatusCode.NotFound));
                }

                _logger.LogInformation("Usuario interno obtenido: {Email}", user.Email);

                return Ok(APIResponse<InternUserDetailsDto>.SuccessResponse(user, 
                    "Usuario interno obtenido exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario interno con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Crea un nuevo usuario interno
        /// </summary>
        /// <param name="createDto">Datos para crear el usuario interno</param>
        /// <returns>Usuario interno creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<InternUserResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateInternUser([FromBody] CreateInternUserDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando usuario interno: {Email}", createDto.Email);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var user = await _internUserService.CreateAsync(createDto);

                _logger.LogInformation("Usuario interno creado exitosamente: {Email} con ID {Id}", 
                    user.Email, user.Id);

                return CreatedAtAction(nameof(GetInternUser), new { id = user.Id }, 
                    APIResponse<InternUserResponseDto>.SuccessResponse(user, "Usuario interno creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al crear usuario interno {Email}: {Error}", 
                    createDto.Email, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario interno: {Email}", createDto.Email);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Actualiza un usuario interno existente
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <param name="updateDto">Datos para actualizar</param>
        /// <returns>Usuario interno actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<InternUserResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateInternUser(int id, [FromBody] UpdateInternUserDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando usuario interno con ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var user = await _internUserService.UpdateAsync(id, updateDto);

                _logger.LogInformation("Usuario interno actualizado exitosamente: {Email}", user.Email);

                return Ok(APIResponse<InternUserResponseDto>.SuccessResponse(user, 
                    "Usuario interno actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al actualizar usuario interno {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando usuario interno con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Elimina un usuario interno (soft delete)
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <returns>True si se eliminó correctamente</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteInternUser(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando usuario interno con ID: {Id}", id);

                var result = await _internUserService.DeleteAsync(id);

                _logger.LogInformation("Usuario interno eliminado exitosamente con ID: {Id}", id);

                return Ok(APIResponse<bool>.SuccessResponse(result, "Usuario interno eliminado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al eliminar usuario interno {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando usuario interno con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Busca usuarios internos por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de usuarios internos que coinciden</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<InternUserListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchInternUsers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(APIResponse.ErrorResponse("El término de búsqueda es obligatorio"));
                }

                _logger.LogInformation("Buscando usuarios internos con término: {SearchTerm}", searchTerm);

                var users = await _internUserService.SearchAsync(searchTerm);

                _logger.LogInformation("Búsqueda completada: {Count} usuarios encontrados", users.Count());

                return Ok(APIResponse<IEnumerable<InternUserListDto>>.SuccessResponse(users, 
                    "Búsqueda completada exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando usuarios internos con término: {SearchTerm}", searchTerm);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene usuarios internos activos
        /// </summary>
        /// <returns>Lista de usuarios internos activos</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<InternUserListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetActiveInternUsers()
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos activos");

                var users = await _internUserService.GetActiveAsync();

                _logger.LogInformation("Usuarios activos obtenidos: {Count}", users.Count());

                return Ok(APIResponse<IEnumerable<InternUserListDto>>.SuccessResponse(users, 
                    "Usuarios activos obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos activos");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtiene usuarios internos por rol
        /// </summary>
        /// <param name="rolId">ID del rol</param>
        /// <returns>Lista de usuarios internos con el rol especificado</returns>
        [HttpGet("by-role/{rolId}")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<InternUserListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetInternUsersByRole(int rolId)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos por rol ID: {RolId}", rolId);

                var users = await _internUserService.GetByRoleAsync(rolId);

                _logger.LogInformation("Usuarios por rol obtenidos: {Count}", users.Count());

                return Ok(APIResponse<IEnumerable<InternUserListDto>>.SuccessResponse(users, 
                    "Usuarios por rol obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos por rol ID: {RolId}", rolId);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario interno
        /// </summary>
        /// <param name="id">ID del usuario interno</param>
        /// <param name="changePasswordDto">Datos para cambiar la contraseña</param>
        /// <returns>True si se cambió correctamente</returns>
        [HttpPost("{id}/change-password")]
        [ProducesResponseType(typeof(APIResponse<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                _logger.LogInformation("Cambiando contraseña para usuario interno con ID: {Id}", id);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(APIResponse.ErrorResponse($"Datos de entrada inválidos: {string.Join(", ", errors)}"));
                }

                var result = await _internUserService.ChangePasswordAsync(id, changePasswordDto);

                _logger.LogInformation("Contraseña cambiada exitosamente para usuario interno con ID: {Id}", id);

                return Ok(APIResponse<bool>.SuccessResponse(result, "Contraseña cambiada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Error de validación al cambiar contraseña para usuario interno {Id}: {Error}", id, ex.Message);
                return BadRequest(APIResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cambiando contraseña para usuario interno con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
