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
            try
            {
                var maintenances = await _projectMaintenanceService.GetAllProjectMaintenancesAsync();
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                    maintenances, 
                    "Mantenimientos de proyectos obtenidos exitosamente", 
                    StatusCodes.Status200OK);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenanceById(int id)
        {
            try
            {
                var idValidation = _projectMaintenanceValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var maintenance = await _projectMaintenanceService.GetProjectMaintenanceByIdAsync(id);
                
                if (maintenance == null)
                {
                    var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                        $"No se encontró un mantenimiento con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<ProjectMaintenanceDataTransferObject>.SuccessResponse(
                    maintenance, 
                    "Mantenimiento obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("project/{projectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetProjectMaintenancesByProject(int projectId)
        {
            try
            {
                var projectIdValidation = _projectMaintenanceValidator.ValidateProjectId(projectId);
                if (!projectIdValidation.IsValid)
                {
                    var errors = projectIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                        "ID de proyecto inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var maintenances = await _projectMaintenanceService.GetProjectMaintenancesByProjectAsync(projectId);
                
                // Si no hay mantenimientos para ese proyecto, retornar mensaje apropiado
                if (!maintenances.Any())
                {
                    var emptyResponse = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                        maintenances, 
                        $"No se encontraron mantenimientos para el proyecto {projectId}", 
                        StatusCodes.Status200OK);
                    return Ok(emptyResponse);
                }
                
                var successResponse = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                    maintenances, 
                    $"Se encontraron {maintenances.Count()} mantenimiento(s) para el proyecto {projectId}", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("employee/{employeeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetProjectMaintenancesByEmployee(int employeeId)
        {
            try
            {
                var employeeIdValidation = _projectMaintenanceValidator.ValidateEmployeeId(employeeId);
                if (!employeeIdValidation.IsValid)
                {
                    var errors = employeeIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                        "ID de empleado inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var maintenances = await _projectMaintenanceService.GetProjectMaintenancesByEmployeeAsync(employeeId);
                
                // Si no hay mantenimientos para ese empleado, retornar mensaje apropiado
                if (!maintenances.Any())
                {
                    var emptyResponse = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                        maintenances, 
                        $"No se encontraron mantenimientos realizados por el empleado {employeeId}", 
                        StatusCodes.Status200OK);
                    return Ok(emptyResponse);
                }
                
                var successResponse = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                    maintenances, 
                    $"Se encontraron {maintenances.Count()} mantenimiento(s) realizados por el empleado {employeeId}", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("state/{stateId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>>> GetProjectMaintenancesByState(int stateId)
        {
            try
            {
                var stateIdValidation = _projectMaintenanceValidator.ValidateStateId(stateId);
                if (!stateIdValidation.IsValid)
                {
                    var errors = stateIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                        "ID de estado inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var maintenances = await _projectMaintenanceService.GetProjectMaintenancesByStateAsync(stateId);
                
                // Si no hay mantenimientos con ese estado, retornar mensaje apropiado
                if (!maintenances.Any())
                {
                    var emptyResponse = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                        maintenances, 
                        $"No se encontraron mantenimientos con el estado {stateId}", 
                        StatusCodes.Status200OK);
                    return Ok(emptyResponse);
                }
                
                var successResponse = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.SuccessResponse(
                    maintenances, 
                    $"Se encontraron {maintenances.Count()} mantenimiento(s) con el estado {stateId}", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectMaintenanceDataTransferObject>>> CreateProjectMaintenance(CreateProjectMaintenanceDataTransferObject createDto)
        {
            try
            {
                var validation = await _projectMaintenanceValidator.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                        "Datos de mantenimiento inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var createdMaintenance = await _projectMaintenanceService.CreateProjectMaintenanceAsync(createDto);
                var successResponse = ApiResponse<ProjectMaintenanceDataTransferObject>.SuccessResponse(
                    createdMaintenance, 
                    "Mantenimiento creado exitosamente", 
                    StatusCodes.Status201Created);
                
                return CreatedAtAction(nameof(GetProjectMaintenanceById), new { id = createdMaintenance.Id }, successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectMaintenanceDataTransferObject>>> UpdateProjectMaintenance(UpdateProjectMaintenanceDataTransferObject updateDto)
        {
            try
            {
                var validation = await _projectMaintenanceValidator.ValidateUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                        "Datos de mantenimiento inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var updatedMaintenance = await _projectMaintenanceService.UpdateProjectMaintenanceAsync(updateDto);
                
                if (updatedMaintenance == null)
                {
                    var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                        $"No se encontró un mantenimiento con ID {updateDto.Id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<ProjectMaintenanceDataTransferObject>.SuccessResponse(
                    updatedMaintenance, 
                    "Mantenimiento actualizado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProjectMaintenance(int id)
        {
            try
            {
                var idValidation = _projectMaintenanceValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<string>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var deleted = await _projectMaintenanceService.DeleteProjectMaintenanceAsync(id);
                
                if (!deleted)
                {
                    var response = ApiResponse<string>.ErrorResponse(
                        $"No se encontró un mantenimiento con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<string>.SuccessResponse(
                    "Mantenimiento eliminado exitosamente", 
                    "Mantenimiento eliminado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
