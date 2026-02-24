using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.State;
using WorkTrackBio.API.Services.StateService;
using WorkTrackBio.API.Validators.StateValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;
        private readonly IStateValidator _stateValidator;

        public StateController(IStateService stateService, IStateValidator stateValidator)
        {
            _stateService = stateService ?? throw new ArgumentNullException(nameof(stateService));
            _stateValidator = stateValidator ?? throw new ArgumentNullException(nameof(stateValidator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<StateDataTransferObject>>>> GetAllStates()
        {
            try
            {
                var states = await _stateService.GetAllStatesAsync();
                var response = ApiResponse<IEnumerable<StateDataTransferObject>>.SuccessResponse(
                    states, 
                    "Estados obtenidos exitosamente", 
                    StatusCodes.Status200OK);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<StateDataTransferObject>>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<StateDataTransferObject>>> GetStateById(int id)
        {
            try
            {
                var idValidation = _stateValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var state = await _stateService.GetStateByIdAsync(id);
                
                if (state == null)
                {
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        $"No se encontró un estado con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<StateDataTransferObject>.SuccessResponse(
                    state, 
                    "Estado obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("type/{stateType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<StateDataTransferObject>>>> GetStatesByType(string stateType)
        {
            try
            {
                var typeValidation = _stateValidator.ValidateStateType(stateType);
                if (!typeValidation.IsValid)
                {
                    var errors = typeValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<StateDataTransferObject>>.ErrorResponse(
                        "Tipo de estado inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var states = await _stateService.GetStatesByTypeAsync(stateType);
                var successResponse = ApiResponse<IEnumerable<StateDataTransferObject>>.SuccessResponse(
                    states, 
                    "Estados obtenidos por tipo exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<StateDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<StateDataTransferObject>>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<StateDataTransferObject>>> CreateState([FromBody] CreateStateDataTransferObject createDto)
        {
            try
            {
                if (createDto == null)
                {
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        "Los datos del estado no pueden estar vacíos", 
                        StatusCodes.Status400BadRequest);
                    return BadRequest(response);
                }

                var validation = await _stateValidator.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        "Datos de entrada inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var createdState = await _stateService.CreateStateAsync(createDto);
                
                var successResponse = ApiResponse<StateDataTransferObject>.SuccessResponse(
                    createdState, 
                    "Estado creado exitosamente", 
                    StatusCodes.Status201Created);
                
                return CreatedAtAction(
                    nameof(GetStateById), 
                    new { id = createdState.Id }, 
                    successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<StateDataTransferObject>>> UpdateState(int id, [FromBody] UpdateStateDataTransferObject updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        "Los datos del estado no pueden estar vacíos", 
                        StatusCodes.Status400BadRequest);
                    return BadRequest(response);
                }

                if (id != updateDto.Id)
                {
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        "El ID de la URL no coincide con el ID del estado", 
                        StatusCodes.Status400BadRequest);
                    return BadRequest(response);
                }

                var validation = await _stateValidator.ValidateUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        "Datos de entrada inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var updatedState = await _stateService.UpdateStateAsync(updateDto);
                
                if (updatedState == null)
                {
                    var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                        $"No se encontró un estado con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<StateDataTransferObject>.SuccessResponse(
                    updatedState, 
                    "Estado actualizado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<StateDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteState(int id)
        {
            try
            {
                var idValidation = _stateValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<object>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var deleted = await _stateService.DeleteStateAsync(id);
                
                if (!deleted)
                {
                    var response = ApiResponse<object>.ErrorResponse(
                        $"No se encontró un estado con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<object>.SuccessResponse(
                    "Estado eliminado exitosamente", 
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

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> StateExists(int id)
        {
            try
            {
                var idValidation = _stateValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _stateService.StateExistsAsync(id);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "El estado existe" : "El estado no existe", 
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

        [HttpGet("name/{stateName}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> StateNameExists(string stateName)
        {
            try
            {
                var nameValidation = _stateValidator.ValidateStateName(stateName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "Nombre de estado inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _stateService.StateNameExistsAsync(stateName);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "Ya existe un estado con ese nombre" : "No existe un estado con ese nombre", 
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
