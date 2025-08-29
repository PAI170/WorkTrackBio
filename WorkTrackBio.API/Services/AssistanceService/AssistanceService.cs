using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Assistance;
using WorkTrackBio.API.Repositories.AssistanceRepository;
using WorkTrackBio.API.Validators.AssistanceValidator;

namespace WorkTrackBio.API.Services.AssistanceService
{
    /// <summary>
    /// Implementación del servicio para Assistance
    /// </summary>
    public class AssistanceService : IAssistanceService
    {
        private readonly IAssistanceRepository _assistanceRepository;
        private readonly IAssistanceValidator _assistanceValidator;
        private readonly IMapper _mapper;

        public AssistanceService(
            IAssistanceRepository assistanceRepository,
            IAssistanceValidator assistanceValidator,
            IMapper mapper)
        {
            _assistanceRepository = assistanceRepository ?? throw new ArgumentNullException(nameof(assistanceRepository));
            _assistanceValidator = assistanceValidator ?? throw new ArgumentNullException(nameof(assistanceValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAllAssistancesAsync()
        {
            try
            {
                var assistances = await _assistanceRepository.GetAllAsync();
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AssistanceDataTransferObject>> GetAssistanceByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<AssistanceDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0");

                var assistance = await _assistanceRepository.GetByIdAsync(id);
                if (assistance == null)
                    return ApiResponse<AssistanceDataTransferObject>.ErrorResponse($"No se encontró un registro de asistencia con ID {id}");

                var assistanceDto = _mapper.Map<AssistanceDataTransferObject>(assistance);

                return ApiResponse<AssistanceDataTransferObject>.SuccessResponse(
                    assistanceDto,
                    "Registro de asistencia encontrado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<AssistanceDataTransferObject>.ErrorResponse(
                    $"Error al obtener el registro de asistencia: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del empleado debe ser mayor que 0");

                var assistances = await _assistanceRepository.GetByEmployeeAsync(employeeId);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para el empleado {employeeId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia del empleado: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAsync(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0");

                var assistances = await _assistanceRepository.GetByProjectAsync(projectId);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para el proyecto {projectId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia del proyecto: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByDateAsync(DateOnly date)
        {
            try
            {
                var assistances = await _assistanceRepository.GetByDateAsync(date);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para la fecha {date:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia por fecha: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                if (startDate > endDate)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("La fecha de inicio no puede ser posterior a la fecha de fin");

                var assistances = await _assistanceRepository.GetByDateRangeAsync(startDate, endDate);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia por rango de fechas: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAndProjectAsync(int employeeId, int projectId)
        {
            try
            {
                if (employeeId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del empleado debe ser mayor que 0");

                if (projectId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0");

                var assistances = await _assistanceRepository.GetByEmployeeAndProjectAsync(employeeId, projectId);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para el empleado {employeeId} en el proyecto {projectId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia por empleado y proyecto: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                if (employeeId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del empleado debe ser mayor que 0");

                if (startDate > endDate)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("La fecha de inicio no puede ser posterior a la fecha de fin");

                var assistances = await _assistanceRepository.GetByEmployeeAndDateRangeAsync(employeeId, startDate, endDate);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para el empleado {employeeId} entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia por empleado y rango de fechas: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAndDateRangeAsync(int projectId, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0");

                if (startDate > endDate)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("La fecha de inicio no puede ser posterior a la fecha de fin");

                var assistances = await _assistanceRepository.GetByProjectAndDateRangeAsync(projectId, startDate, endDate);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para el proyecto {projectId} entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia por proyecto y rango de fechas: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<AssistanceDataTransferObject>>> GetAssistancesByProjectAndEmployeeAsync(int projectId, int employeeId)
        {
            try
            {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0");

                if (employeeId <= 0)
                    return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse("El ID del empleado debe ser mayor que 0");

                var assistances = await _assistanceRepository.GetByProjectAndEmployeeAsync(projectId, employeeId);
                var assistanceDtos = _mapper.Map<IEnumerable<AssistanceDataTransferObject>>(assistances);

                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.SuccessResponse(
                    assistanceDtos,
                    $"Se encontraron {assistanceDtos.Count()} registro(s) de asistencia para el proyecto {projectId} y empleado {employeeId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AssistanceDataTransferObject>>.ErrorResponse(
                    $"Error al obtener los registros de asistencia por proyecto y empleado: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AssistanceDataTransferObject>> CreateAssistanceAsync(CreateAssistanceDataTransferObject createDto)
        {
            try
            {
                            // Validar el DTO
            var validationResult = await _assistanceValidator.ValidateCreateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApiResponse<AssistanceDataTransferObject>.ErrorResponse("Error de validación", 400, errors);
            }

                // Mapear DTO a modelo
                var assistance = _mapper.Map<Assistance>(createDto);

                // Crear el registro
                var createdAssistance = await _assistanceRepository.CreateAsync(assistance);
                var assistanceDto = _mapper.Map<AssistanceDataTransferObject>(createdAssistance);

                return ApiResponse<AssistanceDataTransferObject>.SuccessResponse(
                    assistanceDto,
                    "Registro de asistencia creado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<AssistanceDataTransferObject>.ErrorResponse(
                    $"Error al crear el registro de asistencia: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AssistanceDataTransferObject>> UpdateAssistanceAsync(int id, UpdateAssistanceDataTransferObject updateDto)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<AssistanceDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0");

                // Obtener el registro existente
                var existingAssistance = await _assistanceRepository.GetByIdAsync(id);
                if (existingAssistance == null)
                    return ApiResponse<AssistanceDataTransferObject>.ErrorResponse($"No se encontró un registro de asistencia con ID {id}");

                            // Validar el DTO
            var validationResult = await _assistanceValidator.ValidateUpdateAsync(id, updateDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return ApiResponse<AssistanceDataTransferObject>.ErrorResponse("Error de validación", 400, errors);
            }

                // Crear un HashSet para rastrear qué campos fueron enviados explícitamente
                var sentFields = new HashSet<string>();
                var updateDtoType = typeof(UpdateAssistanceDataTransferObject);
                foreach (var property in updateDtoType.GetProperties())
                {
                    if (property.GetValue(updateDto) != null)
                    {
                        sentFields.Add(property.Name);
                    }
                }

                // Aplicar actualizaciones parciales
                bool hasChanges = false;

                if (sentFields.Contains("EmployeeId"))
                {
                    existingAssistance.EmployeeId = updateDto.EmployeeId!.Value;
                    hasChanges = true;
                }

                if (sentFields.Contains("ProjectId"))
                {
                    existingAssistance.ProjectId = updateDto.ProjectId!.Value;
                    hasChanges = true;
                }

                if (sentFields.Contains("Notes"))
                {
                    existingAssistance.Notes = updateDto.Notes;
                    hasChanges = true;
                }

                if (sentFields.Contains("CheckIn"))
                {
                    existingAssistance.CheckIn = updateDto.CheckIn!.Value;
                    hasChanges = true;
                }

                if (sentFields.Contains("CheckOut"))
                {
                    existingAssistance.CheckOut = updateDto.CheckOut;
                    hasChanges = true;
                }

                if (sentFields.Contains("TotalHours"))
                {
                    existingAssistance.TotalHours = updateDto.TotalHours;
                    hasChanges = true;
                }

                if (sentFields.Contains("RegisterType"))
                {
                    existingAssistance.RegisterType = updateDto.RegisterType!;
                    hasChanges = true;
                }

                if (!hasChanges)
                {
                    return ApiResponse<AssistanceDataTransferObject>.ErrorResponse("No se detectaron cambios para actualizar");
                }

                // Actualizar el registro
                var updatedAssistance = await _assistanceRepository.UpdateAsync(existingAssistance);
                var assistanceDto = _mapper.Map<AssistanceDataTransferObject>(updatedAssistance);

                return ApiResponse<AssistanceDataTransferObject>.SuccessResponse(
                    assistanceDto,
                    "Registro de asistencia actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<AssistanceDataTransferObject>.ErrorResponse(
                    $"Error al actualizar el registro de asistencia: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAssistanceAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0");

                // Verificar que el registro existe
                var existingAssistance = await _assistanceRepository.GetByIdAsync(id);
                if (existingAssistance == null)
                    return ApiResponse<bool>.ErrorResponse($"No se encontró un registro de asistencia con ID {id}");

                // Verificar dependencias
                var hasDependencies = await _assistanceRepository.HasDependenciesAsync(id);
                if (hasDependencies)
                    return ApiResponse<bool>.ErrorResponse("No se puede eliminar el registro de asistencia porque tiene dependencias (registros de auditoría)");

                // Eliminar el registro
                var deleted = await _assistanceRepository.DeleteAsync(id);
                if (!deleted)
                    return ApiResponse<bool>.ErrorResponse("Error al eliminar el registro de asistencia");

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Registro de asistencia eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    $"Error al eliminar el registro de asistencia: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> CalculateTotalHoursAsync(int assistanceId)
        {
            try
            {
                if (assistanceId <= 0)
                    return ApiResponse<decimal>.ErrorResponse("El ID debe ser mayor que 0");

                var assistance = await _assistanceRepository.GetByIdAsync(assistanceId);
                if (assistance == null)
                    return ApiResponse<decimal>.ErrorResponse($"No se encontró un registro de asistencia con ID {assistanceId}");

                if (!assistance.CheckOut.HasValue)
                    return ApiResponse<decimal>.ErrorResponse("No se puede calcular las horas totales sin fecha de salida");

                var totalHours = (decimal)(assistance.CheckOut.Value - assistance.CheckIn).TotalHours;
                totalHours = Math.Round(totalHours, 2);

                return ApiResponse<decimal>.SuccessResponse(
                    totalHours,
                    $"Horas totales calculadas: {totalHours} horas");
            }
            catch (Exception ex)
            {
                return ApiResponse<decimal>.ErrorResponse(
                    $"Error al calcular las horas totales: {ex.Message}");
            }
        }
    }
}
