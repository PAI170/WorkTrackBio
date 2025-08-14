using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Common;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para obtener datos de catálogos/lookups desde la base de datos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LookupsController : ControllerBase
    {
        private readonly ILookupService _lookupService;
        private readonly ILogger<LookupsController> _logger;

        public LookupsController(
            ILookupService lookupService,
            ILogger<LookupsController> logger)
        {
            _lookupService = lookupService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los estados del sistema
        /// </summary>
        /// <returns>Lista de estados</returns>
        [HttpGet("states")]
        [ProducesResponseType(typeof(APIResponse<List<StateDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetStates()
        {
            try
            {
                _logger.LogInformation("Obteniendo estados del sistema");
                
                var states = await _lookupService.GetAllStatesAsync();
                
                var stateDtos = states.Select(s => new StateDto(
                    s.Id,
                    s.StateName,
                    s.StateType,
                    s.Description ?? string.Empty
                )).ToList();

                return Ok(APIResponse<List<StateDto>>.SuccessResponse(
                    stateDtos,
                    $"Se obtuvieron {stateDtos.Count} estados exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener todos los roles de usuario
        /// </summary>
        /// <returns>Lista de roles</returns>
        [HttpGet("roles")]
        [ProducesResponseType(typeof(APIResponse<List<RoleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                _logger.LogInformation("Obteniendo roles de usuario");
                
                var roles = await _lookupService.GetAllRolesAsync();
                
                var roleDtos = roles.Select(r => new RoleDto(
                    r.Id,
                    r.RoleName,
                    r.Description ?? string.Empty
                )).ToList();

                return Ok(APIResponse<List<RoleDto>>.SuccessResponse(
                    roleDtos,
                    $"Se obtuvieron {roleDtos.Count} roles exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener todos los tipos de documento
        /// </summary>
        /// <returns>Lista de tipos de documento</returns>
        [HttpGet("document-types")]
        [ProducesResponseType(typeof(APIResponse<List<DocumentTypeDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDocumentTypes()
        {
            try
            {
                _logger.LogInformation("Obteniendo tipos de documento");
                
                var documentTypes = await _lookupService.GetAllDocumentTypesAsync();
                
                var documentTypeDtos = documentTypes.Select(dt => new DocumentTypeDto(
                    dt.Id,
                    dt.DocumentName,
                    dt.Description ?? string.Empty
                )).ToList();

                return Ok(APIResponse<List<DocumentTypeDto>>.SuccessResponse(
                    documentTypeDtos,
                    $"Se obtuvieron {documentTypeDtos.Count} tipos de documento exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipos de documento");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener estados específicos por tipo
        /// </summary>
        /// <param name="stateType">Tipo de estado (General, Project, Employee)</param>
        /// <returns>Lista de estados filtrados por tipo</returns>
        [HttpGet("states/{stateType}")]
        [ProducesResponseType(typeof(APIResponse<List<StateDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetStatesByType(string stateType)
        {
            try
            {
                _logger.LogInformation("Obteniendo estados por tipo: {StateType}", stateType);
                
                var states = await _lookupService.GetStatesByTypeAsync(stateType);
                
                var stateDtos = states.Select(s => new StateDto(
                    s.Id,
                    s.StateName,
                    s.StateType,
                    s.Description ?? string.Empty
                )).ToList();

                return Ok(APIResponse<List<StateDto>>.SuccessResponse(
                    stateDtos,
                    $"Se obtuvieron {stateDtos.Count} estados del tipo '{stateType}' exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados por tipo: {StateType}", stateType);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener un estado específico por ID
        /// </summary>
        /// <param name="id">ID del estado</param>
        /// <returns>Estado específico</returns>
        [HttpGet("states/id/{id}")]
        [ProducesResponseType(typeof(APIResponse<StateDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetStateById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo estado con ID: {Id}", id);
                
                var state = await _lookupService.GetStateByIdAsync(id);
                
                if (state == null)
                {
                    return NotFound(APIResponse.ErrorResponse("Estado no encontrado", HttpStatusCode.NotFound));
                }
                
                var stateDto = new StateDto(
                    state.Id,
                    state.StateName,
                    state.StateType,
                    state.Description ?? string.Empty
                );

                return Ok(APIResponse<StateDto>.SuccessResponse(
                    stateDto,
                    "Estado obtenido exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener un rol específico por ID
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Rol específico</returns>
        [HttpGet("roles/{id}")]
        [ProducesResponseType(typeof(APIResponse<RoleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo rol con ID: {Id}", id);
                
                var role = await _lookupService.GetRoleByIdAsync(id);
                
                if (role == null)
                {
                    return NotFound(APIResponse.ErrorResponse("Rol no encontrado", HttpStatusCode.NotFound));
                }
                
                var roleDto = new RoleDto(
                    role.Id,
                    role.RoleName,
                    role.Description ?? string.Empty
                );

                return Ok(APIResponse<RoleDto>.SuccessResponse(
                    roleDto,
                    "Rol obtenido exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener un tipo de documento específico por ID
        /// </summary>
        /// <param name="id">ID del tipo de documento</param>
        /// <returns>Tipo de documento específico</returns>
        [HttpGet("document-types/{id}")]
        [ProducesResponseType(typeof(APIResponse<DocumentTypeDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetDocumentTypeById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo tipo de documento con ID: {Id}", id);
                
                var documentType = await _lookupService.GetDocumentTypeByIdAsync(id);
                
                if (documentType == null)
                {
                    return NotFound(APIResponse.ErrorResponse("Tipo de documento no encontrado", HttpStatusCode.NotFound));
                }
                
                var documentTypeDto = new DocumentTypeDto(
                    documentType.Id,
                    documentType.DocumentName,
                    documentType.Description ?? string.Empty
                );

                return Ok(APIResponse<DocumentTypeDto>.SuccessResponse(
                    documentTypeDto,
                    "Tipo de documento obtenido exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipo de documento con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener todos los lookups del sistema en una sola llamada
        /// </summary>
        /// <returns>Todos los datos de lookup del sistema</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllLookups()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los lookups del sistema");
                
                var lookupData = await _lookupService.GetAllLookupsAsync();
                
                var response = new
                {
                    States = lookupData.States.Select(s => new StateDto(s.Id, s.StateName, s.StateType, s.Description ?? string.Empty)),
                    Roles = lookupData.Roles.Select(r => new RoleDto(r.Id, r.RoleName, r.Description ?? string.Empty)),
                    DocumentTypes = lookupData.DocumentTypes.Select(dt => new DocumentTypeDto(dt.Id, dt.DocumentName, dt.Description ?? string.Empty))
                };

                return Ok(APIResponse<object>.SuccessResponse(
                    response,
                    "Todos los lookups obtenidos exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los lookups");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
