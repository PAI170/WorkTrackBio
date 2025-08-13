using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Data.Repositories.Models;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de asistencia que encapsula la lógica de negocio
    /// </summary>
    public class AssistanceService : IAssistanceService
    {
        private readonly IAssistanceRepository _assistanceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssistanceService> _logger;

        public AssistanceService(
            IAssistanceRepository assistanceRepository,
            IEmployeeRepository employeeRepository,
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            ILogger<AssistanceService> logger)
        {
            _assistanceRepository = assistanceRepository;
            _employeeRepository = employeeRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Operaciones CRUD Básicas

        public async Task<Assistance?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencia con ID: {AssistanceId}", id);
                return await _assistanceRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia con ID: {AssistanceId}", id);
                throw;
            }
        }

        public async Task<Assistance?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencia con ID: {AssistanceId} e includes", id);
                var result = await _assistanceRepository.GetByIdWithIncludesAsync(id,
                    a => a.Employee,
                    a => a.Project,
                    a => a.Device);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia con ID: {AssistanceId} e includes", id);
                throw;
            }
        }

        public async Task<IEnumerable<Assistance>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las asistencias");
                return await _assistanceRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las asistencias");
                throw;
            }
        }

        public async Task<Assistance> CreateAsync(CheckInDto checkInDto)
        {
            try
            {
                _logger.LogInformation("Creando nueva asistencia para empleado: {EmployeeId}", checkInDto.EmployeeId);

                // Validar que el empleado exista
                var employee = await _employeeRepository.GetByIdAsync(checkInDto.EmployeeId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"No se encontró el empleado con ID: {checkInDto.EmployeeId}");
                }

                // Validar que el proyecto exista
                var project = await _projectRepository.GetByIdAsync(checkInDto.ProjectId);
                if (project == null)
                {
                    throw new InvalidOperationException($"No se encontró el proyecto con ID: {checkInDto.ProjectId}");
                }

                // Validar que no tenga asistencia activa
                if (await _assistanceRepository.HasActiveAssistanceAsync(checkInDto.EmployeeId))
                {
                    throw new InvalidOperationException($"El empleado ya tiene una asistencia activa");
                }

                // Crear la entidad
                var assistance = new Assistance
                {
                    EmployeeId = checkInDto.EmployeeId,
                    ProjectId = checkInDto.ProjectId,
                    CheckIn = checkInDto.CheckIn,
                    RegisterType = "Manual", // Valor por defecto para registros manuales
                    Notes = checkInDto.Notes,
                    DeviceId = checkInDto.DeviceId
                };

                var createdAssistance = await _assistanceRepository.AddAsync(assistance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Asistencia creada exitosamente con ID: {AssistanceId}", createdAssistance.Id);
                return createdAssistance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear asistencia para empleado: {EmployeeId}", checkInDto.EmployeeId);
                throw;
            }
        }

        public async Task<Assistance> UpdateAsync(int id, UpdateAssistanceDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando asistencia con ID: {AssistanceId}", id);

                var existingAssistance = await _assistanceRepository.GetByIdAsync(id);
                if (existingAssistance == null)
                {
                    throw new InvalidOperationException($"No se encontró la asistencia con ID: {id}");
                }

                // Actualizar propiedades
                existingAssistance.Notes = updateDto.Notes;
                existingAssistance.RegisterType = updateDto.RegisterType;

                var updatedAssistance = await _assistanceRepository.UpdateAsync(existingAssistance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Asistencia actualizada exitosamente con ID: {AssistanceId}", id);
                return updatedAssistance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar asistencia con ID: {AssistanceId}", id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando asistencia con ID: {AssistanceId}", id);

                var assistance = await _assistanceRepository.GetByIdAsync(id);
                if (assistance == null)
                {
                    throw new InvalidOperationException($"No se encontró la asistencia con ID: {id}");
                }

                // Validar que la asistencia pueda ser eliminada
                var (isValid, errors) = await ValidateForDeletionAsync(id);
                if (!isValid)
                {
                    throw new InvalidOperationException($"No se puede eliminar la asistencia: {string.Join(", ", errors)}");
                }

                await _assistanceRepository.DeleteAsync(assistance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Asistencia eliminada exitosamente con ID: {AssistanceId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar asistencia con ID: {AssistanceId}", id);
                throw;
            }
        }

        #endregion

        #region Operaciones de Búsqueda y Filtrado

        public async Task<(IEnumerable<Assistance> Items, int TotalCount)> GetFilteredAsync(AssistanceFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencias filtradas");
                return await _assistanceRepository.GetFilteredAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias filtradas");
                throw;
            }
        }

        public async Task<IEnumerable<Assistance>> GetByEmployeeAndPeriodAsync(int employeeId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencias del empleado {EmployeeId} del {FromDate} al {ToDate}", 
                    employeeId, fromDate, toDate);
                return await _assistanceRepository.GetByEmployeeAndPeriodAsync(employeeId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias del empleado {EmployeeId} del {FromDate} al {ToDate}", 
                    employeeId, fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<Assistance>> GetByProjectAndPeriodAsync(int projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencias del proyecto {ProjectId} del {FromDate} al {ToDate}", 
                    projectId, fromDate, toDate);
                return await _assistanceRepository.GetByProjectAndPeriodAsync(projectId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias del proyecto {ProjectId} del {FromDate} al {ToDate}", 
                    projectId, fromDate, toDate);
                throw;
            }
        }

        #endregion

        #region Operaciones Específicas del Negocio

        public async Task<Assistance?> GetActiveAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencia activa del empleado: {EmployeeId}", employeeId);
                return await _assistanceRepository.GetActiveAssistanceAsync(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencia activa del empleado: {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<bool> HasActiveAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Verificando si el empleado {EmployeeId} tiene asistencia activa", employeeId);
                return await _assistanceRepository.HasActiveAssistanceAsync(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar si el empleado {EmployeeId} tiene asistencia activa", employeeId);
                throw;
            }
        }

        public async Task<Assistance> CheckInAsync(CheckInDto checkInDto)
        {
            try
            {
                _logger.LogInformation("Registrando check-in para empleado: {EmployeeId}", checkInDto.EmployeeId);

                // Validar que no tenga asistencia activa
                if (await _assistanceRepository.HasActiveAssistanceAsync(checkInDto.EmployeeId))
                {
                    throw new InvalidOperationException($"El empleado ya tiene una asistencia activa");
                }

                // Crear asistencia
                var assistance = new Assistance
                {
                    EmployeeId = checkInDto.EmployeeId,
                    ProjectId = checkInDto.ProjectId,
                    CheckIn = checkInDto.CheckIn,
                    RegisterType = "Manual", // Valor por defecto para registros manuales
                    Notes = checkInDto.Notes,
                    DeviceId = checkInDto.DeviceId
                };

                var createdAssistance = await _assistanceRepository.AddAsync(assistance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Check-in registrado exitosamente para empleado: {EmployeeId}", checkInDto.EmployeeId);
                return createdAssistance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar check-in para empleado: {EmployeeId}", checkInDto.EmployeeId);
                throw;
            }
        }

        public async Task<Assistance> CheckOutAsync(CheckOutDto checkOutDto)
        {
            try
            {
                _logger.LogInformation("Registrando check-out para asistencia: {AssistanceId}", checkOutDto.AssistanceId);

                // Obtener asistencia por ID
                var assistance = await _assistanceRepository.GetByIdAsync(checkOutDto.AssistanceId);
                if (assistance == null)
                {
                    throw new InvalidOperationException($"No se encontró la asistencia con ID: {checkOutDto.AssistanceId}");
                }

                // Validar que la asistencia no tenga check-out previo
                if (assistance.CheckOut.HasValue)
                {
                    throw new InvalidOperationException($"La asistencia ya tiene un check-out registrado");
                }

                // Validar que el check-out sea posterior al check-in
                if (checkOutDto.CheckOut <= assistance.CheckIn)
                {
                    throw new InvalidOperationException($"El check-out debe ser posterior al check-in");
                }

                // Calcular horas trabajadas
                var timeWorked = checkOutDto.CheckOut - assistance.CheckIn;
                var totalHours = (decimal)timeWorked.TotalHours;

                // Actualizar asistencia
                assistance.CheckOut = checkOutDto.CheckOut;
                assistance.TotalHours = totalHours;
                assistance.Notes = checkOutDto.Notes ?? assistance.Notes;

                var updatedAssistance = await _assistanceRepository.UpdateAsync(assistance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Check-out registrado exitosamente para asistencia: {AssistanceId}", checkOutDto.AssistanceId);
                return updatedAssistance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar check-out para asistencia: {AssistanceId}", checkOutDto.AssistanceId);
                throw;
            }
        }

        public async Task<IEnumerable<Assistance>> GetByWorkHoursRangeAsync(decimal minHours, decimal maxHours, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencias por rango de horas: {MinHours} - {MaxHours}", minHours, maxHours);
                return await _assistanceRepository.GetByWorkHoursRangeAsync(minHours, maxHours, fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias por rango de horas: {MinHours} - {MaxHours}", minHours, maxHours);
                throw;
            }
        }

        public async Task<IEnumerable<Assistance>> GetExcessiveWorkTimeAsync(DateTime fromDate, DateTime toDate, int maxHours = 12)
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencias con tiempo excesivo (más de {MaxHours} horas)", maxHours);
                return await _assistanceRepository.GetExcessiveWorkTimeAsync(fromDate, toDate, maxHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias con tiempo excesivo (más de {MaxHours} horas)", maxHours);
                throw;
            }
        }

        #endregion

        #region Operaciones de Reportes y Estadísticas

        public async Task<IEnumerable<EmployeeWorkSummary>> GetEmployeeWorkSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo resumen de trabajo por empleado del {FromDate} al {ToDate}", fromDate, toDate);
                return await _assistanceRepository.GetEmployeeWorkSummaryAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de trabajo por empleado del {FromDate} al {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectWorkSummary>> GetProjectWorkSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo resumen de trabajo por proyecto del {FromDate} al {ToDate}", fromDate, toDate);
                return await _assistanceRepository.GetProjectWorkSummaryAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de trabajo por proyecto del {FromDate} al {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<AssistanceStatistics> GetStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de asistencia del {FromDate} al {ToDate}", fromDate, toDate);
                return await _assistanceRepository.GetAssistanceStatisticsAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de asistencia del {FromDate} al {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<TopWorkerSummary>> GetTopWorkersAsync(DateTime fromDate, DateTime toDate, int top = 10)
        {
            try
            {
                _logger.LogInformation("Obteniendo top {Top} trabajadores del {FromDate} al {ToDate}", top, fromDate, toDate);
                return await _assistanceRepository.GetTopWorkersAsync(fromDate, toDate, top);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top {Top} trabajadores del {FromDate} al {ToDate}", top, fromDate, toDate);
                throw;
            }
        }

        #endregion

        #region Operaciones de Validación y Corrección

        public async Task<IEnumerable<Assistance>> GetNeedingCorrectionAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo asistencias que requieren corrección");
                return await _assistanceRepository.GetAssistancesNeedingCorrectionAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asistencias que requieren corrección");
                throw;
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(CheckInDto checkInDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el empleado exista
                var employee = await _employeeRepository.GetByIdAsync(checkInDto.EmployeeId);
                if (employee == null)
                {
                    errors.Add($"No se encontró el empleado con ID: {checkInDto.EmployeeId}");
                }

                // Validar que el proyecto exista
                var project = await _projectRepository.GetByIdAsync(checkInDto.ProjectId);
                if (project == null)
                {
                    errors.Add($"No se encontró el proyecto con ID: {checkInDto.ProjectId}");
                }

                // Validar que no tenga asistencia activa
                if (await _assistanceRepository.HasActiveAssistanceAsync(checkInDto.EmployeeId))
                {
                    errors.Add("El empleado ya tiene una asistencia activa");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar asistencia para creación");
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(int id, UpdateAssistanceDto updateDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que la asistencia exista
                var assistance = await _assistanceRepository.GetByIdAsync(id);
                if (assistance == null)
                {
                    errors.Add($"No se encontró la asistencia con ID: {id}");
                    return (false, errors);
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar asistencia para actualización con ID: {AssistanceId}", id);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForDeletionAsync(int id)
        {
            var errors = new List<string>();

            try
            {
                // Validar que la asistencia exista
                var assistance = await _assistanceRepository.GetByIdAsync(id);
                if (assistance == null)
                {
                    errors.Add($"No se encontró la asistencia con ID: {id}");
                    return (false, errors);
                }

                // Aquí podrías agregar más validaciones de negocio
                // Por ejemplo, verificar que no esté en un período de nómina cerrado

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar asistencia para eliminación con ID: {AssistanceId}", id);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateCheckInAsync(CheckInDto checkInDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el empleado exista
                var employee = await _employeeRepository.GetByIdAsync(checkInDto.EmployeeId);
                if (employee == null)
                {
                    errors.Add($"No se encontró el empleado con ID: {checkInDto.EmployeeId}");
                }

                // Validar que el proyecto exista
                var project = await _projectRepository.GetByIdAsync(checkInDto.ProjectId);
                if (project == null)
                {
                    errors.Add($"No se encontró el proyecto con ID: {checkInDto.ProjectId}");
                }

                // Validar que no tenga asistencia activa
                if (await _assistanceRepository.HasActiveAssistanceAsync(checkInDto.EmployeeId))
                {
                    errors.Add("El empleado ya tiene una asistencia activa");
                }

                // Validar que el check-in no sea en el futuro
                if (checkInDto.CheckIn > DateTime.UtcNow)
                {
                    errors.Add("El check-in no puede ser en el futuro");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar check-in");
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateCheckOutAsync(CheckOutDto checkOutDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que la asistencia exista
                var assistance = await _assistanceRepository.GetByIdAsync(checkOutDto.AssistanceId);
                if (assistance == null)
                {
                    errors.Add($"No se encontró la asistencia con ID: {checkOutDto.AssistanceId}");
                    return (false, errors);
                }

                // Validar que la asistencia no tenga check-out previo
                if (assistance.CheckOut.HasValue)
                {
                    errors.Add("La asistencia ya tiene un check-out registrado");
                }

                // Validar que el check-out no sea en el futuro
                if (checkOutDto.CheckOut > DateTime.UtcNow)
                {
                    errors.Add("El check-out no puede ser en el futuro");
                }

                // Validar que el check-out sea posterior al check-in
                if (checkOutDto.CheckOut <= assistance.CheckIn)
                {
                    errors.Add("El check-out debe ser posterior al check-in");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar check-out");
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        #endregion
    }
}
