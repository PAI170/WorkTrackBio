using Microsoft.AspNetCore.Mvc;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Helpers;
using System.Net;

namespace WTB.API.Controllers
{
    /// <summary>
    /// Controller básico para gestión de empleados
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ILogger<EmployeesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Crear un nuevo empleado
        /// </summary>
        /// <param name="dto">Datos del empleado a crear</param>
        /// <returns>Respuesta con el empleado creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<EmployeeResponseDto>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.BadRequest)]
        public IActionResult CreateEmployee([FromBody] CreateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Creando empleado: {DocumentNumber}", dto.DocumentNumber);

                // Simulamos la creación del empleado
                var employeeResponse = new EmployeeResponseDto(
                    Id: new Random().Next(1, 1000),
                    DocumentNumber: dto.DocumentNumber,
                    DocumentTypeId: dto.DocumentTypeId,
                    DocumentTypeName: GetDocumentTypeName(dto.DocumentTypeId),
                    DocumentExpire: dto.DocumentExpire,
                    FirstName: dto.FirstName,
                    LastName: dto.LastName,
                    FullName: $"{dto.FirstName} {dto.LastName}",
                    PhoneNumber: dto.PhoneNumber,
                    EmergencyContact: dto.EmergencyContact,
                    EmergencyContactPhoneNumber: dto.EmergencyContactPhoneNumber,
                    Birthday: dto.Birthday,
                    Age: CalculateAge(dto.Birthday),
                    RegisterDate: DateTime.UtcNow,
                    CostPerHour: dto.CostPerHour,
                    StateId: dto.StateId,
                    StateName: GetStateName(dto.StateId),
                    Address: dto.Address,
                    IBAN: dto.IBAN,
                    HasFingerPrint: false,
                    TotalProjects: 0,
                    LastAssistance: null
                );

                var response = APIResponse<EmployeeResponseDto>.SuccessResponse(
                    employeeResponse,
                    "Empleado creado exitosamente"
                );
                response.StatusCode = HttpStatusCode.Created;

                return CreatedAtAction(nameof(GetEmployeeById), new { id = employeeResponse.Id }, response);
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
        public IActionResult UpdateEmployee(int id, [FromBody] UpdateEmployeeDto dto)
        {
            try
            {
                _logger.LogInformation("Actualizando empleado: {Id}", id);

                // Validar que el ID del DTO coincida con el de la URL
                if (dto.Id != id)
                {
                    return BadRequest(APIResponse.ErrorResponse("El ID del empleado no coincide con la URL"));
                }

                // Simular que el empleado no existe (para demostrar NotFound)
                if (id <= 0)
                {
                    return NotFound(APIResponse.ErrorResponse("Empleado no encontrado", HttpStatusCode.NotFound));
                }

                // Simulamos la actualización del empleado
                var employeeResponse = new EmployeeResponseDto(
                    Id: dto.Id,
                    DocumentNumber: dto.DocumentNumber,
                    DocumentTypeId: dto.DocumentTypeId,
                    DocumentTypeName: GetDocumentTypeName(dto.DocumentTypeId),
                    DocumentExpire: dto.DocumentExpire,
                    FirstName: dto.FirstName,
                    LastName: dto.LastName,
                    FullName: $"{dto.FirstName} {dto.LastName}",
                    PhoneNumber: dto.PhoneNumber,
                    EmergencyContact: dto.EmergencyContact,
                    EmergencyContactPhoneNumber: dto.EmergencyContactPhoneNumber,
                    Birthday: dto.Birthday,
                    Age: CalculateAge(dto.Birthday),
                    RegisterDate: DateTime.UtcNow.AddDays(-30), // Simular fecha de registro anterior
                    CostPerHour: dto.CostPerHour,
                    StateId: dto.StateId,
                    StateName: GetStateName(dto.StateId),
                    Address: dto.Address,
                    IBAN: dto.IBAN,
                    HasFingerPrint: new Random().Next(0, 2) == 1,
                    TotalProjects: new Random().Next(1, 5),
                    LastAssistance: DateTime.UtcNow.AddDays(-new Random().Next(1, 7))
                );

                return Ok(APIResponse<EmployeeResponseDto>.SuccessResponse(
                    employeeResponse,
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
        /// Obtener un empleado por ID
        /// </summary>
        /// <param name="id">ID del empleado</param>
        /// <returns>Datos del empleado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<EmployeeResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleado con ID: {Id}", id);

                if (id <= 0)
                {
                    return NotFound(APIResponse.ErrorResponse("Empleado no encontrado", HttpStatusCode.NotFound));
                }

                // Simulamos un empleado
                var employee = new EmployeeResponseDto(
                    Id: id,
                    DocumentNumber: "1-1234-5678",
                    DocumentTypeId: 1,
                    DocumentTypeName: "Cédula de Identidad",
                    DocumentExpire: DateTime.Today.AddYears(5),
                    FirstName: "Juan",
                    LastName: "Pérez",
                    FullName: "Juan Pérez",
                    PhoneNumber: "2456-7890",
                    EmergencyContact: "María Pérez",
                    EmergencyContactPhoneNumber: "2456-7891",
                    Birthday: new DateTime(1990, 5, 15),
                    Age: CalculateAge(new DateTime(1990, 5, 15)),
                    RegisterDate: DateTime.UtcNow.AddDays(-30),
                    CostPerHour: 3500.00m,
                    StateId: 1,
                    StateName: "Active",
                    Address: "San José, Costa Rica",
                    IBAN: "CR05015202001234567890",
                    HasFingerPrint: true,
                    TotalProjects: 3,
                    LastAssistance: DateTime.UtcNow.AddDays(-2)
                );

                return Ok(APIResponse<EmployeeResponseDto>.SuccessResponse(
                    employee,
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
        /// Obtener lista de empleados con filtros
        /// </summary>
        /// <param name="filter">Filtros de búsqueda</param>
        /// <returns>Lista paginada de empleados</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<EmployeeListDto>), (int)HttpStatusCode.OK)]
        public IActionResult GetEmployees([FromQuery] EmployeeFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de empleados con filtros");

                // Simulamos una lista de empleados
                var employees = new List<EmployeeListDto>
                {
                    new EmployeeListDto(1, "1-1234-5678", "Juan Pérez", "2456-7890", "Cédula de Identidad", "Active", 3500.00m, DateTime.UtcNow.AddDays(-30), true),
                    new EmployeeListDto(2, "2-2345-6789", "María González", "2567-8901", "Cédula de Identidad", "Active", 3200.00m, DateTime.UtcNow.AddDays(-45), true),
                    new EmployeeListDto(3, "DIM-12345678", "Carlos Rodríguez", "2678-9012", "DIMEX", "Active", 3800.00m, DateTime.UtcNow.AddDays(-60), true),
                    new EmployeeListDto(4, "PT-123456", "Ana Morales", "2789-0123", "Permiso de Trabajo", "Inactive", 3000.00m, DateTime.UtcNow.AddDays(-90), false)
                };

                // Aplicar filtros básicos (simulados)
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    employees = employees.Where(e => 
                        e.FullName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        e.DocumentNumber.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (filter.IsActive.HasValue)
                {
                    employees = employees.Where(e => e.IsActive == filter.IsActive.Value).ToList();
                }

                // Paginación simulada
                var totalRecords = employees.Count;
                var pagedEmployees = employees
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToList();

                var response = PagedResponse<EmployeeListDto>.Create(
                    pagedEmployees,
                    filter.Page,
                    filter.PageSize,
                    totalRecords,
                    "Empleados obtenidos exitosamente"
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
        /// Eliminar un empleado (soft delete)
        /// </summary>
        /// <param name="id">ID del empleado</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse), (int)HttpStatusCode.NotFound)]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando empleado con ID: {Id}", id);

                if (id <= 0)
                {
                    return NotFound(APIResponse.ErrorResponse("Empleado no encontrado", HttpStatusCode.NotFound));
                }

                // Simulamos la eliminación
                return Ok(APIResponse.SuccessResponse($"Empleado con ID {id} eliminado exitosamente"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empleado con ID: {Id}", id);
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

        /// <summary>
        /// Obtiene el nombre del tipo de documento (simulado)
        /// </summary>
        private static string GetDocumentTypeName(int documentTypeId) => documentTypeId switch
        {
            1 => "Cédula de Identidad",
            2 => "Pasaporte",
            3 => "DIMEX",
            4 => "Permiso de Trabajo",
            _ => "Desconocido"
        };

        /// <summary>
        /// Obtiene el nombre del estado (simulado)
        /// </summary>
        private static string GetStateName(int stateId) => stateId switch
        {
            1 => "Active",
            2 => "Inactive",
            3 => "Suspended",
            _ => "Desconocido"
        };

        #endregion
    }
}
