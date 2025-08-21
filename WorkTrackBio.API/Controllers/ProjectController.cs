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
        private readonly IProjectValidator _projectValidator;

        public ProjectController(IProjectService projectService, IProjectValidator projectValidator)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _projectValidator = projectValidator ?? throw new ArgumentNullException(nameof(projectValidator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDataTransferObject>>>> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjectsAsync();
                var response = ApiResponse<IEnumerable<ProjectDataTransferObject>>.SuccessResponse(
                    projects, 
                    "Proyectos obtenidos exitosamente", 
                    StatusCodes.Status200OK);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ProjectDataTransferObject>>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> GetProjectById(int id)
        {
            try
            {
                var idValidation = _projectValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var project = await _projectService.GetProjectByIdAsync(id);
                
                if (project == null)
                {
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        $"No se encontró un proyecto con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<ProjectDataTransferObject>.SuccessResponse(
                    project, 
                    "Proyecto obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("name/{projectName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> GetProjectByName(string projectName)
        {
            try
            {
                var nameValidation = _projectValidator.ValidateProjectName(projectName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        "Nombre de proyecto inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var project = await _projectService.GetProjectByNameAsync(projectName);
                
                if (project == null)
                {
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        $"No se encontró un proyecto con el nombre '{projectName}'", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<ProjectDataTransferObject>.SuccessResponse(
                    project, 
                    "Proyecto obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectDataTransferObject>>>> GetProjectsByState(int stateId)
        {
            try
            {
                var stateIdValidation = _projectValidator.ValidateId(stateId);
                if (!stateIdValidation.IsValid)
                {
                    var errors = stateIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<ProjectDataTransferObject>>.ErrorResponse(
                        "ID de estado inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var projects = await _projectService.GetProjectsByStateAsync(stateId);
                
                // Si no hay proyectos con ese estado, retornar mensaje apropiado
                if (!projects.Any())
                {
                    var emptyResponse = ApiResponse<IEnumerable<ProjectDataTransferObject>>.SuccessResponse(
                        projects, 
                        $"No se encontraron proyectos con el estado {stateId}", 
                        StatusCodes.Status200OK);
                    return Ok(emptyResponse);
                }
                
                var successResponse = ApiResponse<IEnumerable<ProjectDataTransferObject>>.SuccessResponse(
                    projects, 
                    $"Se encontraron {projects.Count()} proyecto(s) con el estado {stateId}", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<ProjectDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<IEnumerable<ProjectDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> ProjectExists(int id)
        {
            try
            {
                var idValidation = _projectValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _projectService.ProjectExistsAsync(id);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "El proyecto existe" : "El proyecto no existe", 
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

        [HttpGet("name/{projectName}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> ProjectNameExists(string projectName)
        {
            try
            {
                var nameValidation = _projectValidator.ValidateProjectName(projectName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "Nombre de proyecto inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _projectService.ProjectNameExistsAsync(projectName);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? $"Ya existe un proyecto con el nombre '{projectName}'" : $"No existe un proyecto con el nombre '{projectName}'", 
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> CreateProject(CreateProjectDataTransferObject createDto)
        {
            try
            {
                var validation = await _projectValidator.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        "Datos de proyecto inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var createdProject = await _projectService.CreateProjectAsync(createDto);
                var successResponse = ApiResponse<ProjectDataTransferObject>.SuccessResponse(
                    createdProject, 
                    "Proyecto creado exitosamente", 
                    StatusCodes.Status201Created);
                
                return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<ProjectDataTransferObject>>> UpdateProject(UpdateProjectDataTransferObject updateDto)
        {
            try
            {
                var validation = await _projectValidator.ValidateUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        "Datos de proyecto inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var updatedProject = await _projectService.UpdateProjectAsync(updateDto);
                
                if (updatedProject == null)
                {
                    var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                        $"No se encontró un proyecto con ID {updateDto.Id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<ProjectDataTransferObject>.SuccessResponse(
                    updatedProject, 
                    "Proyecto actualizado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProjectDataTransferObject>.ErrorResponse(
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteProject(int id)
        {
            try
            {
                var idValidation = _projectValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<string>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var deleted = await _projectService.DeleteProjectAsync(id);
                
                if (!deleted)
                {
                    var response = ApiResponse<string>.ErrorResponse(
                        $"No se encontró un proyecto con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<string>.SuccessResponse(
                    "Proyecto eliminado exitosamente", 
                    "Proyecto eliminado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
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
