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
            var response = await _employeeInfoService.GetAllEmployeesAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> GetEmployeeById(int id)
        {
            var response = await _employeeInfoService.GetEmployeeByIdAsync(id);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("document/{documentNumber}/{documentTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> GetEmployeeByDocument(string documentNumber, int documentTypeId)
        {
            var response = await _employeeInfoService.GetEmployeeByDocumentNumberAsync(documentNumber, documentTypeId);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("state/{stateId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>>> GetEmployeesByState(int stateId)
        {
            var response = await _employeeInfoService.GetEmployeesByStateAsync(stateId);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("documenttype/{documentTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>>> GetEmployeesByDocumentType(int documentTypeId)
        {
            var response = await _employeeInfoService.GetEmployeesByDocumentTypeAsync(documentTypeId);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> EmployeeExists(int id)
        {
            var response = await _employeeInfoService.EmployeeExistsAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("document/{documentNumber}/{documentTypeId:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> EmployeeDocumentExists(string documentNumber, int documentTypeId)
        {
            var response = await _employeeInfoService.EmployeeDocumentExistsAsync(documentNumber, documentTypeId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> CreateEmployee(CreateEmployeeInfoDataTransferObject createDto)
        {
            var validation = await _employeeInfoValidator.ValidateCreateAsync(createDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("Datos de empleado inválidos", 400, errors));
            }

            var response = await _employeeInfoService.CreateEmployeeAsync(createDto);

            if (!response.Success)
            {
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetEmployeeById), new { id = response.Data!.Id }, response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> UpdateEmployee(UpdateEmployeeInfoDataTransferObject updateDto)
        {
            var validation = await _employeeInfoValidator.ValidateUpdateAsync(updateDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<EmployeeInfoDataTransferObject>.ErrorResponse("Datos de empleado inválidos", 400, errors));
            }

            var response = await _employeeInfoService.UpdateEmployeeAsync(updateDto);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteEmployee(int id)
        {
            var response = await _employeeInfoService.DeleteEmployeeAsync(id);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}