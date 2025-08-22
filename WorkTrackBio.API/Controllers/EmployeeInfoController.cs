using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.EmployeeInfo;
using WorkTrackBio.API.Services.EmployeeInfoService;
using WorkTrackBio.API.Validators.EmployeeInfoValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeInfoController : ControllerBase
    {
        private readonly IEmployeeInfoService _employeeInfoService;
        private readonly IEmployeeInfoValidator _employeeInfoValidator;

        public EmployeeInfoController(IEmployeeInfoService employeeInfoService, IEmployeeInfoValidator employeeInfoValidator)
        {
            _employeeInfoService = employeeInfoService ?? throw new ArgumentNullException(nameof(employeeInfoService));
            _employeeInfoValidator = employeeInfoValidator ?? throw new ArgumentNullException(nameof(employeeInfoValidator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>>> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeInfoService.GetAllEmployeesAsync();
                var response = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(
                    employees, 
                    "Empleados obtenidos exitosamente", 
                    StatusCodes.Status200OK);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> GetEmployeeById(int id)
        {
            try
            {
                var idValidation = _employeeInfoValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var employee = await _employeeInfoService.GetEmployeeByIdAsync(id);
                
                if (employee == null)
                {
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        $"No se encontró un empleado con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(
                    employee, 
                    "Empleado obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("document/{documentNumber}/{documentTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> GetEmployeeByDocument(string documentNumber, int documentTypeId)
        {
            try
            {
                var documentNumberValidation = _employeeInfoValidator.ValidateDocumentNumber(documentNumber);
                if (!documentNumberValidation.IsValid)
                {
                    var errors = documentNumberValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        "Número de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var documentTypeIdValidation = _employeeInfoValidator.ValidateId(documentTypeId);
                if (!documentTypeIdValidation.IsValid)
                {
                    var errors = documentTypeIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        "ID del tipo de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var employee = await _employeeInfoService.GetEmployeeByDocumentNumberAsync(documentNumber, documentTypeId);
                
                if (employee == null)
                {
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        $"No se encontró un empleado con el número de documento '{documentNumber}' del tipo especificado", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(
                    employee, 
                    "Empleado obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>>> GetEmployeesByState(int stateId)
        {
            try
            {
                var stateIdValidation = _employeeInfoValidator.ValidateId(stateId);
                if (!stateIdValidation.IsValid)
                {
                    var errors = stateIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
                        "ID de estado inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var employees = await _employeeInfoService.GetEmployeesByStateAsync(stateId);
                
                // Si no hay empleados con ese estado, retornar mensaje apropiado
                if (!employees.Any())
                {
                    var emptyResponse = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(
                        employees, 
                        $"No se encontraron empleados con el estado {stateId}", 
                        StatusCodes.Status200OK);
                    return Ok(emptyResponse);
                }
                
                var successResponse = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(
                    employees, 
                    $"Se encontraron {employees.Count()} empleado(s) con el estado {stateId}", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        [HttpGet("documenttype/{documentTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>>> GetEmployeesByDocumentType(int documentTypeId)
        {
            try
            {
                var documentTypeIdValidation = _employeeInfoValidator.ValidateId(documentTypeId);
                if (!documentTypeIdValidation.IsValid)
                {
                    var errors = documentTypeIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
                        "ID del tipo de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var employees = await _employeeInfoService.GetEmployeesByDocumentTypeAsync(documentTypeId);
                
                // Si no hay empleados con ese tipo de documento, retornar mensaje apropiado
                if (!employees.Any())
                {
                    var emptyResponse = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(
                        employees, 
                        $"No se encontraron empleados con el tipo de documento {documentTypeId}", 
                        StatusCodes.Status200OK);
                    return Ok(emptyResponse);
                }
                
                var successResponse = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.SuccessResponse(
                    employees, 
                    $"Se encontraron {employees.Count()} empleado(s) con el tipo de documento {documentTypeId}", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<bool>>> EmployeeExists(int id)
        {
            try
            {
                var idValidation = _employeeInfoValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _employeeInfoService.EmployeeExistsAsync(id);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "El empleado existe" : "El empleado no existe", 
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

        [HttpGet("document/{documentNumber}/{documentTypeId:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> EmployeeDocumentExists(string documentNumber, int documentTypeId)
        {
            try
            {
                var documentNumberValidation = _employeeInfoValidator.ValidateDocumentNumber(documentNumber);
                if (!documentNumberValidation.IsValid)
                {
                    var errors = documentNumberValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "Número de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var documentTypeIdValidation = _employeeInfoValidator.ValidateId(documentTypeId);
                if (!documentTypeIdValidation.IsValid)
                {
                    var errors = documentTypeIdValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "ID del tipo de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _employeeInfoService.EmployeeDocumentExistsAsync(documentNumber, documentTypeId);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? $"Ya existe un empleado con el número de documento '{documentNumber}' del tipo especificado" : $"No existe un empleado con el número de documento '{documentNumber}' del tipo especificado", 
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
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> CreateEmployee(CreateEmployeeInfoDataTransferObject createDto)
        {
            try
            {
                var validation = await _employeeInfoValidator.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        "Datos de empleado inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var createdEmployee = await _employeeInfoService.CreateEmployeeAsync(createDto);
                var successResponse = ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(
                    createdEmployee, 
                    "Empleado creado exitosamente", 
                    StatusCodes.Status201Created);
                
                return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.Id }, successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> UpdateEmployee(UpdateEmployeeInfoDataTransferObject updateDto)
        {
            try
            {
                var validation = await _employeeInfoValidator.ValidateUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        "Datos de empleado inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var updatedEmployee = await _employeeInfoService.UpdateEmployeeAsync(updateDto);
                
                if (updatedEmployee == null)
                {
                    var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                        $"No se encontró un empleado con ID {updateDto.Id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<EmployeeInfoDataTransferObject>.SuccessResponse(
                    updatedEmployee, 
                    "Empleado actualizado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse(
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
        public async Task<ActionResult<ApiResponse<string>>> DeleteEmployee(int id)
        {
            try
            {
                var idValidation = _employeeInfoValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<string>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var deleted = await _employeeInfoService.DeleteEmployeeAsync(id);
                
                if (!deleted)
                {
                    var response = ApiResponse<string>.ErrorResponse(
                        $"No se encontró un empleado con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<string>.SuccessResponse(
                    "Empleado eliminado exitosamente", 
                    "Empleado eliminado exitosamente", 
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
