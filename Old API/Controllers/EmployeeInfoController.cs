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

        public EmployeeInfoController(IEmployeeInfoService employeeInfoService, IEmployeeInfoValidator employeeInfoValidator)
        {
            _employeeInfoService = employeeInfoService ?? throw new ArgumentNullException(nameof(employeeInfoService));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EmployeeInfoDataTransferObject>>>> GetAllEmployees()
        {
            var response = await _employeeInfoService.GetAllEmployeesAsync();
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return Ok (response); 
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> GetEmployeeById(int id)
        {
            var response = await _employeeInfoService.GetEmployeeByIdAsync(id);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> CreateEmployee(CreateEmployeeInfoDataTransferObject createDto)
        {
            var response = await _employeeInfoService.CreateEmployeeAsync(createDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = response.Data!.Id }, response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<EmployeeInfoDataTransferObject>>> UpdateEmployee(UpdateEmployeeInfoDataTransferObject updateDto)
        {
            var response = await _employeeInfoService.UpdateEmployeeAsync(updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}