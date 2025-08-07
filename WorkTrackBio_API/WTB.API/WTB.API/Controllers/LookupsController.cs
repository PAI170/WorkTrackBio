using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Common;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controller para obtener datos de catálogos/lookups
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class LookupsController : ControllerBase
    {
        private readonly ILogger<LookupsController> _logger;

        public LookupsController(ILogger<LookupsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtener todos los estados del sistema
        /// </summary>
        /// <returns>Lista de estados</returns>
        [HttpGet("states")]
        [ProducesResponseType(typeof(APIResponse<List<StateDto>>), (int)HttpStatusCode.OK)]
        public IActionResult GetStates()
        {
            try
            {
                _logger.LogInformation("Obteniendo estados del sistema");

                var states = new List<StateDto>
                {
                    new StateDto(1, "Active", "General", "Estado activo del sistema"),
                    new StateDto(2, "Inactive", "General", "Estado inactivo del sistema"),
                    new StateDto(3, "Suspended", "General", "Estado suspendido temporalmente"),
                    new StateDto(4, "In Progress", "Project", "Proyecto en progreso"),
                    new StateDto(5, "Completed", "Project", "Proyecto completado"),
                    new StateDto(6, "On Hold", "Project", "Proyecto en pausa"),
                    new StateDto(7, "Pending", "Employee", "Empleado pendiente de aprobación"),
                    new StateDto(8, "Terminated", "Employee", "Empleado dado de baja")
                };

                return Ok(APIResponse<List<StateDto>>.SuccessResponse(
                    states,
                    "Estados obtenidos exitosamente"
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
        public IActionResult GetRoles()
        {
            try
            {
                _logger.LogInformation("Obteniendo roles de usuario");

                var roles = new List<RoleDto>
                {
                    new RoleDto(1, "Administrator", "Administrador del sistema con acceso completo"),
                    new RoleDto(2, "HR Manager", "Gerente de recursos humanos"),
                    new RoleDto(3, "Project Manager", "Gerente de proyectos"),
                    new RoleDto(4, "Supervisor", "Supervisor de campo"),
                    new RoleDto(5, "Operator", "Operador básico del sistema")
                };

                return Ok(APIResponse<List<RoleDto>>.SuccessResponse(
                    roles,
                    "Roles obtenidos exitosamente"
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
        public IActionResult GetDocumentTypes()
        {
            try
            {
                _logger.LogInformation("Obteniendo tipos de documento");

                var documentTypes = new List<DocumentTypeDto>
                {
                    new DocumentTypeDto(1, "Cedula de Identidad", "Cédula de identidad costarricense"),
                    new DocumentTypeDto(2, "Pasaporte", "Pasaporte internacional"),
                    new DocumentTypeDto(3, "DIMEX", "Documento de Identidad Migratoria para Extranjeros"),
                    new DocumentTypeDto(4, "Permiso de Trabajo", "Permiso de trabajo para extranjeros")
                };

                return Ok(APIResponse<List<DocumentTypeDto>>.SuccessResponse(
                    documentTypes,
                    "Tipos de documento obtenidos exitosamente"
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
        public IActionResult GetStatesByType(string stateType)
        {
            try
            {
                _logger.LogInformation("Obteniendo estados por tipo: {StateType}", stateType);

                var allStates = new List<StateDto>
                {
                    new StateDto(1, "Active", "General", "Estado activo del sistema"),
                    new StateDto(2, "Inactive", "General", "Estado inactivo del sistema"),
                    new StateDto(3, "Suspended", "General", "Estado suspendido temporalmente"),
                    new StateDto(4, "In Progress", "Project", "Proyecto en progreso"),
                    new StateDto(5, "Completed", "Project", "Proyecto completado"),
                    new StateDto(6, "On Hold", "Project", "Proyecto en pausa"),
                    new StateDto(7, "Pending", "Employee", "Empleado pendiente de aprobación"),
                    new StateDto(8, "Terminated", "Employee", "Empleado dado de baja")
                };

                var filteredStates = allStates
                    .Where(s => s.StateType.Equals(stateType, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return Ok(APIResponse<List<StateDto>>.SuccessResponse(
                    filteredStates,
                    $"Estados de tipo '{stateType}' obtenidos exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados por tipo: {StateType}", stateType);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }
    }
}
