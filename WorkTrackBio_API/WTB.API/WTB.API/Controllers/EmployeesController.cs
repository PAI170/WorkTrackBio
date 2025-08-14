using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de empleados conectado a la base de datos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(
            IEmployeeService employeeService,
            ILogger<EmployeesController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener lista de empleados con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de empleados</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<EmployeeListDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetEmployees([FromQuery] EmployeeFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de empleados con filtros");
                
                var (items, totalCount) = await _employeeService.GetFilteredAsync(filter);
                
                // Convertir entidades a DTOs
                var employeeDtos = items.Select(e => new EmployeeListDto(
                    e.Id,
                    e.DocumentNumber,
                    $"{e.FirstName} {e.LastName}",
                    e.PhoneNumber ?? "N/A",
                    e.DocumentType?.DocumentName ?? "N/A",
                    e.State?.StateName ?? "N/A",
                    e.CostPerHour ?? 0,
                    e.CreatedDate,
                    e.FingerPrints.Any()
                ));
                
                var response = APIResponse<IEnumerable<EmployeeListDto>>.SuccessResponse(
                    employeeDtos, 
                    $"Se encontraron {totalCount} empleados"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener lista de empleados");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener un empleado por ID
        /// </summary>
        /// <param name="id">ID del empleado</param>
        /// <returns>Datos del empleado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<EmployeeResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleado con ID: {Id}", id);
                
                var result = await _employeeService.GetByIdWithIncludesAsync(id);
                
                if (result == null)
                {
                    return NotFound(APIResponse.ErrorResponse("Empleado no encontrado", HttpStatusCode.NotFound));
                }
                
                var employeeDto = new EmployeeResponseDto(
                    result.Id,
                    result.DocumentNumber,
                    result.DocumentTypeId,
                    result.DocumentType?.DocumentName ?? "N/A",
                    result.DocumentExpire,
                    result.FirstName,
                    result.LastName,
                    $"{result.FirstName} {result.LastName}",
                    result.PhoneNumber ?? "N/A",
                    result.EmergencyContact ?? "N/A",
                    result.EmergencyContactPhoneNumber ?? "N/A",
                    result.Birthday,
                    CalculateAge(result.Birthday),
                    result.RegisterDate,
                    result.CostPerHour ?? 0,
                    result.StateId,
                    result.State?.StateName ?? "N/A",
                    result.Address ?? "N/A",
                    result.IBAN ?? "N/A",
                    result.FingerPrints.Any(),
                    result.ProjectsAssigns.Count,
                    result.Assistances.OrderByDescending(a => a.CreatedDate).FirstOrDefault()?.CreatedDate
                );
                
                return Ok(APIResponse<EmployeeResponseDto>.SuccessResponse(
                    employeeDto, 
                    "Empleado obtenido exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleado con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Crear un nuevo empleado
        /// </summary>
        /// <param name="dto">Datos del empleado a crear</param>
        /// <returns>Respuesta con el empleado creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<EmployeeResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Creando empleado: {DocumentNumber}", dto.DocumentNumber);

                // Validar antes de crear
                var (isValid, errors) = await _employeeService.ValidateForCreationAsync(dto);
                if (!isValid)
                {
                    return BadRequest(APIResponse.ErrorResponse($"Error de validación: {string.Join(", ", errors)}"));
                }

                var result = await _employeeService.CreateAsync(dto);
                
                var employeeDto = new EmployeeResponseDto(
                    result.Id,
                    result.DocumentNumber,
                    result.DocumentTypeId,
                    result.DocumentType?.DocumentName ?? "N/A",
                    result.DocumentExpire,
                    result.FirstName,
                    result.LastName,
                    $"{result.FirstName} {result.LastName}",
                    result.PhoneNumber ?? "N/A",
                    result.EmergencyContact ?? "N/A",
                    result.EmergencyContactPhoneNumber ?? "N/A",
                    result.Birthday,
                    CalculateAge(result.Birthday),
                    result.RegisterDate,
                    result.CostPerHour ?? 0,
                    result.StateId,
                    result.State?.StateName ?? "N/A",
                    result.Address ?? "N/A",
                    result.IBAN ?? "N/A",
                    result.FingerPrints.Any(),
                    result.ProjectsAssigns.Count,
                    result.Assistances.OrderByDescending(a => a.CreatedDate).FirstOrDefault()?.CreatedDate
                );

                var response = APIResponse<EmployeeResponseDto>.SuccessResponse(
                    employeeDto,
                    "Empleado creado exitosamente"
                );

                return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear empleado");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Actualizar un empleado existente
        /// </summary>
        /// <param name="id">ID del empleado</param>
        /// <param name="dto">Datos actualizados del empleado</param>
        /// <returns>Respuesta con el empleado actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(APIResponse<EmployeeResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Actualizando empleado: {Id}", id);

                // Validar que el ID del DTO coincida con el de la URL
                if (dto.Id != id)
                {
                    return BadRequest(APIResponse.ErrorResponse("El ID del empleado no coincide con la URL"));
                }

                // Validar antes de actualizar
                var (isValid, errors) = await _employeeService.ValidateForUpdateAsync(id, dto);
                if (!isValid)
                {
                    return BadRequest(APIResponse.ErrorResponse($"Error de validación: {string.Join(", ", errors)}"));
                }

                var result = await _employeeService.UpdateAsync(id, dto);
                
                var employeeDto = new EmployeeResponseDto(
                    result.Id,
                    result.DocumentNumber,
                    result.DocumentTypeId,
                    result.DocumentType?.DocumentName ?? "N/A",
                    result.DocumentExpire,
                    result.FirstName,
                    result.LastName,
                    $"{result.FirstName} {result.LastName}",
                    result.PhoneNumber ?? "N/A",
                    result.EmergencyContact ?? "N/A",
                    result.EmergencyContactPhoneNumber ?? "N/A",
                    result.Birthday,
                    CalculateAge(result.Birthday),
                    result.RegisterDate,
                    result.CostPerHour ?? 0,
                    result.StateId,
                    result.State?.StateName ?? "N/A",
                    result.Address ?? "N/A",
                    result.IBAN ?? "N/A",
                    result.FingerPrints.Any(),
                    result.ProjectsAssigns.Count,
                    result.Assistances.OrderByDescending(a => a.CreatedDate).FirstOrDefault()?.CreatedDate
                );

                return Ok(APIResponse<EmployeeResponseDto>.SuccessResponse(
                    employeeDto,
                    "Empleado actualizado exitosamente"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar empleado con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Eliminar un empleado (soft delete)
        /// </summary>
        /// <param name="id">ID del empleado</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando empleado con ID: {Id}", id);

                // Validar antes de eliminar
                var (isValid, errors) = await _employeeService.ValidateForDeletionAsync(id);
                if (!isValid)
                {
                    return BadRequest(APIResponse.ErrorResponse($"No se puede eliminar el empleado: {string.Join(", ", errors)}"));
                }

                await _employeeService.DeleteAsync(id);

                return Ok(APIResponse.SuccessResponse($"Empleado con ID {id} eliminado exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empleado con ID: {Id}", id);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Buscar empleados por término de búsqueda
        /// </summary>
        /// <param name="searchTerm">Término de búsqueda</param>
        /// <returns>Lista de empleados encontrados</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<EmployeeListDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchEmployees([FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando empleados con término: {SearchTerm}", searchTerm);
                
                var results = await _employeeService.SearchAsync(searchTerm);
                
                var employeeDtos = results.Select(e => new EmployeeListDto(
                    e.Id,
                    e.DocumentNumber,
                    $"{e.FirstName} {e.LastName}",
                    e.PhoneNumber ?? "N/A",
                    e.DocumentType?.DocumentName ?? "N/A",
                    e.State?.StateName ?? "N/A",
                    e.CostPerHour ?? 0,
                    e.CreatedDate,
                    e.FingerPrints.Any()
                ));
                
                var response = APIResponse<IEnumerable<EmployeeListDto>>.SuccessResponse(
                    employeeDtos, 
                    $"Se encontraron {employeeDtos.Count()} empleados"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar empleados con término: {SearchTerm}", searchTerm);
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Obtener empleados activos
        /// </summary>
        /// <returns>Lista de empleados activos</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<EmployeeListDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActiveEmployees()
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados activos");
                
                var results = await _employeeService.GetActiveAsync();
                
                var employeeDtos = results.Select(e => new EmployeeListDto(
                    e.Id,
                    e.DocumentNumber,
                    $"{e.FirstName} {e.LastName}",
                    e.PhoneNumber ?? "N/A",
                    e.DocumentType?.DocumentName ?? "N/A",
                    e.State?.StateName ?? "N/A",
                    e.CostPerHour ?? 0,
                    e.CreatedDate,
                    e.FingerPrints.Any()
                ));
                
                var response = APIResponse<IEnumerable<EmployeeListDto>>.SuccessResponse(
                    employeeDtos, 
                    $"Se encontraron {employeeDtos.Count()} empleados activos"
                );
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados activos");
                return StatusCode(500, APIResponse.ErrorResponse("Error interno del servidor", HttpStatusCode.InternalServerError));
            }
        }

        #region Helper Methods

        /// <summary>
        /// Calcula la edad basada en la fecha de nacimiento
        /// </summary>
        private static int CalculateAge(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age))
                age--;
            return age;
        }

        #endregion
    }
}
