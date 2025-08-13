using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Data.Repositories.Models;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de empleados que encapsula la lógica de negocio
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Operaciones CRUD Básicas

        public async Task<EmployeeInfo?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleado con ID: {EmployeeId}", id);
                return await _employeeRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleado con ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task<EmployeeInfo?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleado con ID: {EmployeeId} e includes", id);
                return await _employeeRepository.GetByIdWithIncludesAsync(id, 
                    e => e.DocumentType, 
                    e => e.State, 
                    e => e.FingerPrints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleado con ID: {EmployeeId} e includes", id);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los empleados");
                return await _employeeRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los empleados");
                throw;
            }
        }

        public async Task<EmployeeInfo> CreateAsync(CreateEmployeeDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando nuevo empleado con documento: {DocumentNumber}", createDto.DocumentNumber);

                // Validar que no exista un empleado con el mismo número de documento
                if (await _employeeRepository.ExistsByDocumentNumberAsync(createDto.DocumentNumber))
                {
                    throw new InvalidOperationException($"Ya existe un empleado con el número de documento: {createDto.DocumentNumber}");
                }

                // Crear la entidad
                var employee = new EmployeeInfo
                {
                    DocumentNumber = createDto.DocumentNumber,
                    DocumentTypeId = createDto.DocumentTypeId,
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    Birthday = createDto.Birthday,
                    CostPerHour = createDto.CostPerHour,
                    StateId = createDto.StateId,
                    RegisterDate = DateTime.UtcNow
                };

                var createdEmployee = await _employeeRepository.AddAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Empleado creado exitosamente con ID: {EmployeeId}", createdEmployee.Id);
                return createdEmployee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear empleado con documento: {DocumentNumber}", createDto.DocumentNumber);
                throw;
            }
        }

        public async Task<EmployeeInfo> UpdateAsync(int id, UpdateEmployeeDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando empleado con ID: {EmployeeId}", id);

                var existingEmployee = await _employeeRepository.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    throw new InvalidOperationException($"No se encontró el empleado con ID: {id}");
                }

                // Validar que el nuevo número de documento no esté en uso por otro empleado
                if (updateDto.DocumentNumber != existingEmployee.DocumentNumber &&
                    await _employeeRepository.ExistsByDocumentNumberAsync(updateDto.DocumentNumber))
                {
                    throw new InvalidOperationException($"Ya existe otro empleado con el número de documento: {updateDto.DocumentNumber}");
                }

                // Actualizar propiedades
                existingEmployee.DocumentNumber = updateDto.DocumentNumber;
                existingEmployee.DocumentTypeId = updateDto.DocumentTypeId;
                existingEmployee.FirstName = updateDto.FirstName;
                existingEmployee.LastName = updateDto.LastName;
                existingEmployee.Birthday = updateDto.Birthday;
                existingEmployee.CostPerHour = updateDto.CostPerHour;
                existingEmployee.StateId = updateDto.StateId;
                existingEmployee.PhoneNumber = updateDto.PhoneNumber;
                existingEmployee.Address = updateDto.Address;
                existingEmployee.IBAN = updateDto.IBAN;

                var updatedEmployee = await _employeeRepository.UpdateAsync(existingEmployee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Empleado actualizado exitosamente con ID: {EmployeeId}", id);
                return updatedEmployee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar empleado con ID: {EmployeeId}", id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando empleado con ID: {EmployeeId}", id);

                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                {
                    throw new InvalidOperationException($"No se encontró el empleado con ID: {id}");
                }

                // Validar que el empleado pueda ser eliminado
                var (isValid, errors) = await ValidateForDeletionAsync(id);
                if (!isValid)
                {
                    throw new InvalidOperationException($"No se puede eliminar el empleado: {string.Join(", ", errors)}");
                }

                await _employeeRepository.DeleteAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Empleado eliminado exitosamente con ID: {EmployeeId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar empleado con ID: {EmployeeId}", id);
                throw;
            }
        }

        #endregion

        #region Operaciones de Búsqueda y Filtrado

        public async Task<(IEnumerable<EmployeeInfo> Items, int TotalCount)> GetFilteredAsync(EmployeeFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados filtrados con término: {SearchTerm}", filter.SearchTerm);
                return await _employeeRepository.GetFilteredAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados filtrados");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando empleados con término: {SearchTerm}", searchTerm);
                return await _employeeRepository.SearchByNameAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar empleados con término: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByDocumentTypeAsync(int documentTypeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados por tipo de documento: {DocumentTypeId}", documentTypeId);
                return await _employeeRepository.GetByDocumentTypeAsync(documentTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados por tipo de documento: {DocumentTypeId}", documentTypeId);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByStateAsync(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados por estado: {StateId}", stateId);
                return await _employeeRepository.GetByStateAsync(stateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados por estado: {StateId}", stateId);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetActiveAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados activos");
                return await _employeeRepository.GetActiveEmployeesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados activos");
                throw;
            }
        }

        #endregion

        #region Operaciones Específicas del Negocio

        public async Task<EmployeeInfo?> GetByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleado por número de documento: {DocumentNumber}", documentNumber);
                return await _employeeRepository.GetByDocumentNumberAsync(documentNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleado por número de documento: {DocumentNumber}", documentNumber);
                throw;
            }
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
        {
            try
            {
                _logger.LogInformation("Verificando existencia de empleado con documento: {DocumentNumber}", documentNumber);
                return await _employeeRepository.ExistsByDocumentNumberAsync(documentNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de empleado con documento: {DocumentNumber}", documentNumber);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetWithFingerPrintAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados con huella dactilar");
                return await _employeeRepository.GetEmployeesWithFingerPrintAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados con huella dactilar");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByAgeRangeAsync(int minAge, int maxAge)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados por rango de edad: {MinAge} - {MaxAge}", minAge, maxAge);
                return await _employeeRepository.GetByAgeRangeAsync(minAge, maxAge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados por rango de edad: {MinAge} - {MaxAge}", minAge, maxAge);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByCostPerHourRangeAsync(decimal minCost, decimal maxCost)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados por rango de costo por hora: {MinCost} - {MaxCost}", minCost, maxCost);
                return await _employeeRepository.GetByCostPerHourRangeAsync(minCost, maxCost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados por rango de costo por hora: {MinCost} - {MaxCost}", minCost, maxCost);
                throw;
            }
        }

        #endregion

        #region Operaciones de Estadísticas y Reportes

        public async Task<EmployeeStatistics> GetStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de empleados");
                return await _employeeRepository.GetEmployeeStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de empleados");
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetWithoutAssistanceAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados sin asistencia del {FromDate} al {ToDate}", fromDate, toDate);
                return await _employeeRepository.GetEmployeesWithoutAssistanceAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados sin asistencia del {FromDate} al {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeWorkSummary>> GetTopWorkingAsync(DateTime fromDate, DateTime toDate, int top = 10)
        {
            try
            {
                _logger.LogInformation("Obteniendo top {Top} empleados trabajadores del {FromDate} al {ToDate}", top, fromDate, toDate);
                return await _employeeRepository.GetTopWorkingEmployeesAsync(fromDate, toDate, top);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top {Top} empleados trabajadores del {FromDate} al {ToDate}", top, fromDate, toDate);
                throw;
            }
        }

        #endregion

        #region Operaciones de Validación

        public async Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(CreateEmployeeDto createDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el número de documento no esté en uso
                if (await _employeeRepository.ExistsByDocumentNumberAsync(createDto.DocumentNumber))
                {
                    errors.Add($"Ya existe un empleado con el número de documento: {createDto.DocumentNumber}");
                }

                // Validar edad mínima (18 años)
                var age = DateTime.Today.Year - createDto.Birthday.Year;
                if (age < 18)
                {
                    errors.Add("El empleado debe tener al menos 18 años");
                }

                // Validar que el costo por hora sea positivo
                if (createDto.CostPerHour <= 0)
                {
                    errors.Add("El costo por hora debe ser mayor que cero");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar empleado para creación");
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(int id, UpdateEmployeeDto updateDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el empleado exista
                var existingEmployee = await _employeeRepository.GetByIdAsync(id);
                if (existingEmployee == null)
                {
                    errors.Add($"No se encontró el empleado con ID: {id}");
                    return (false, errors);
                }

                // Validar que el nuevo número de documento no esté en uso por otro empleado
                if (updateDto.DocumentNumber != existingEmployee.DocumentNumber &&
                    await _employeeRepository.ExistsByDocumentNumberAsync(updateDto.DocumentNumber))
                {
                    errors.Add($"Ya existe otro empleado con el número de documento: {updateDto.DocumentNumber}");
                }

                // Validar edad mínima
                var age = DateTime.Today.Year - updateDto.Birthday.Year;
                if (age < 18)
                {
                    errors.Add("El empleado debe tener al menos 18 años");
                }

                // Validar que el costo por hora sea positivo
                if (updateDto.CostPerHour <= 0)
                {
                    errors.Add("El costo por hora debe ser mayor que cero");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar empleado para actualización con ID: {EmployeeId}", id);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForDeletionAsync(int id)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el empleado exista
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null)
                {
                    errors.Add($"No se encontró el empleado con ID: {id}");
                    return (false, errors);
                }

                // Aquí podrías agregar más validaciones de negocio
                // Por ejemplo, verificar que no tenga asistencias activas
                // o que no esté asignado a proyectos activos

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar empleado para eliminación con ID: {EmployeeId}", id);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        #endregion
    }
}
