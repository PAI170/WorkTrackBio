using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Helpers;
using WTB.API.Services.Interfaces;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de asignaciones de proyectos
    /// </summary>
    public class ProjectAssignService : IProjectAssignService
    {
        private readonly IProjectAssignRepository _projectAssignRepository;
        private readonly ILogger<ProjectAssignService> _logger;

        public ProjectAssignService(
            IProjectAssignRepository projectAssignRepository,
            ILogger<ProjectAssignService> logger)
        {
            _projectAssignRepository = projectAssignRepository;
            _logger = logger;
        }

        public async Task<PagedResponse<ProjectAssignListDto>> GetFilteredAsync(ProjectAssignFilterDto filter)
        {
            try
            {
                var pagedResult = await _projectAssignRepository.GetFilteredAsync(filter);
                
                var mappedData = pagedResult.Data?.Select(MapToProjectAssignListDto).ToList() ?? new List<ProjectAssignListDto>();
                
                return PagedResponse<ProjectAssignListDto>.Create(
                    mappedData, 
                    pagedResult.TotalRecords, 
                    filter.Page, 
                    filter.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones filtradas");
                throw;
            }
        }

        public async Task<ProjectAssignDetailsDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _projectAssignRepository.GetByIdWithIncludesAsync(id);
                if (entity == null)
                    return null;

                return MapToProjectAssignDetailsDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignación con ID: {Id}", id);
                throw;
            }
        }

        public async Task<ProjectAssignResponseDto> CreateAsync(CreateProjectAssignDto createDto)
        {
            try
            {
                // Validar datos de entrada
                var validation = await ValidateForCreationAsync(createDto);
                if (!validation.IsValid)
                {
                    throw new InvalidOperationException(validation.ErrorMessage);
                }

                // Verificar si ya existe una asignación activa
                var isAlreadyAssigned = await _projectAssignRepository.IsEmployeeAssignedToProjectAsync(
                    createDto.EmployeeId, createDto.ProjectId);
                
                if (isAlreadyAssigned)
                {
                    throw new InvalidOperationException("El empleado ya está asignado a este proyecto");
                }

                var entity = new ProjectsAssigns
                {
                    EmployeeId = createDto.EmployeeId,
                    ProjectId = createDto.ProjectId,
                    AssignDate = createDto.AssignDate,
                    EndDate = createDto.EndDate
                };

                var createdEntity = await _projectAssignRepository.AddAsync(entity);
                await _projectAssignRepository.SaveChangesAsync();

                _logger.LogInformation("Asignación creada exitosamente con ID: {Id}", createdEntity.Id);

                // Obtener la entidad con includes para mapear a DTO
                var entityWithIncludes = await _projectAssignRepository.GetByIdWithIncludesAsync(createdEntity.Id);
                return MapToProjectAssignResponseDto(entityWithIncludes!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando asignación");
                throw;
            }
        }

        public async Task<ProjectAssignResponseDto> UpdateAsync(UpdateProjectAssignDto updateDto)
        {
            try
            {
                var entity = await _projectAssignRepository.GetByIdAsync(updateDto.Id);
                if (entity == null)
                {
                    throw new InvalidOperationException($"Asignación con ID {updateDto.Id} no encontrada");
                }

                // Validar datos de entrada
                var validation = await ValidateForUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    throw new InvalidOperationException(validation.ErrorMessage);
                }

                // Verificar si ya existe otra asignación activa para el mismo empleado y proyecto
                if (entity.EmployeeId != updateDto.EmployeeId || entity.ProjectId != updateDto.ProjectId)
                {
                    var isAlreadyAssigned = await _projectAssignRepository.IsEmployeeAssignedToProjectAsync(
                        updateDto.EmployeeId, updateDto.ProjectId);
                    
                    if (isAlreadyAssigned)
                    {
                        throw new InvalidOperationException("El empleado ya está asignado a este proyecto");
                    }
                }

                entity.EmployeeId = updateDto.EmployeeId;
                entity.ProjectId = updateDto.ProjectId;
                entity.AssignDate = updateDto.AssignDate;
                entity.EndDate = updateDto.EndDate;

                await _projectAssignRepository.UpdateAsync(entity);
                await _projectAssignRepository.SaveChangesAsync();

                _logger.LogInformation("Asignación actualizada exitosamente con ID: {Id}", entity.Id);

                // Obtener la entidad con includes para mapear a DTO
                var entityWithIncludes = await _projectAssignRepository.GetByIdWithIncludesAsync(entity.Id);
                return MapToProjectAssignResponseDto(entityWithIncludes!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando asignación con ID: {Id}", updateDto.Id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await _projectAssignRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return false;
                }

                // Validar si se puede eliminar
                var validation = await ValidateForDeletionAsync(id);
                if (!validation.CanDelete)
                {
                    throw new InvalidOperationException(validation.ErrorMessage);
                }

                await _projectAssignRepository.DeleteAsync(entity);
                await _projectAssignRepository.SaveChangesAsync();

                _logger.LogInformation("Asignación eliminada exitosamente con ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando asignación con ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectAssignResponseDto>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                var entities = await _projectAssignRepository.GetByEmployeeAsync(employeeId);
                return entities.Select(MapToProjectAssignResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones del empleado con ID: {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectAssignResponseDto>> GetByProjectAsync(int projectId)
        {
            try
            {
                var entities = await _projectAssignRepository.GetByProjectAsync(projectId);
                return entities.Select(MapToProjectAssignResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones del proyecto con ID: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectAssignResponseDto>> GetActiveAsync()
        {
            try
            {
                var entities = await _projectAssignRepository.GetActiveAsync();
                return entities.Select(MapToProjectAssignResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones activas");
                throw;
            }
        }

        public async Task<IEnumerable<ProjectAssignResponseDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var entities = await _projectAssignRepository.GetByDateRangeAsync(fromDate, toDate);
                return entities.Select(MapToProjectAssignResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectAssignResponseDto>> GetByProjectStateAsync(string projectState)
        {
            try
            {
                var entities = await _projectAssignRepository.GetByProjectStateAsync(projectState);
                return entities.Select(MapToProjectAssignResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por estado del proyecto: {ProjectState}", projectState);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectAssignResponseDto>> GetByEmployeeStateAsync(string employeeState)
        {
            try
            {
                var entities = await _projectAssignRepository.GetByEmployeeStateAsync(employeeState);
                return entities.Select(MapToProjectAssignResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por estado del empleado: {EmployeeState}", employeeState);
                throw;
            }
        }

        public async Task<bool> IsEmployeeAssignedToProjectAsync(int employeeId, int projectId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                return await _projectAssignRepository.IsEmployeeAssignedToProjectAsync(employeeId, projectId, fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando si empleado {EmployeeId} está asignado al proyecto {ProjectId}", employeeId, projectId);
                throw;
            }
        }

        public async Task<ProjectAssignStatsDto> GetStatisticsAsync()
        {
            try
            {
                return await _projectAssignRepository.GetStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de asignaciones");
                throw;
            }
        }

        public async Task<ProjectAssignResponseDto> EndAssignmentAsync(int id, DateTime endDate)
        {
            try
            {
                var entity = await _projectAssignRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new InvalidOperationException($"Asignación con ID {id} no encontrada");
                }

                if (entity.EndDate.HasValue)
                {
                    throw new InvalidOperationException("La asignación ya tiene fecha de fin");
                }

                if (endDate <= entity.AssignDate)
                {
                    throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de asignación");
                }

                entity.EndDate = endDate;
                await _projectAssignRepository.UpdateAsync(entity);
                await _projectAssignRepository.SaveChangesAsync();

                _logger.LogInformation("Asignación finalizada exitosamente con ID: {Id}, fecha de fin: {EndDate}", id, endDate);

                var entityWithIncludes = await _projectAssignRepository.GetByIdWithIncludesAsync(id);
                return MapToProjectAssignResponseDto(entityWithIncludes!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizando asignación con ID: {Id}", id);
                throw;
            }
        }

        public async Task<ProjectAssignResponseDto> ReactivateAssignmentAsync(int id)
        {
            try
            {
                var entity = await _projectAssignRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    throw new InvalidOperationException($"Asignación con ID {id} no encontrada");
                }

                if (!entity.EndDate.HasValue)
                {
                    throw new InvalidOperationException("La asignación ya está activa");
                }

                entity.EndDate = null;
                await _projectAssignRepository.UpdateAsync(entity);
                await _projectAssignRepository.SaveChangesAsync();

                _logger.LogInformation("Asignación reactivada exitosamente con ID: {Id}", id);

                var entityWithIncludes = await _projectAssignRepository.GetByIdWithIncludesAsync(id);
                return MapToProjectAssignResponseDto(entityWithIncludes!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reactivando asignación con ID: {Id}", id);
                throw;
            }
        }

        #region Private Methods

        private Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateProjectAssignDto createDto)
        {
            try
            {
                if (createDto.AssignDate < DateTime.UtcNow.AddDays(-1))
                {
                    return Task.FromResult((false, "La fecha de asignación no puede ser anterior a ayer"));
                }

                if (createDto.EndDate.HasValue && createDto.EndDate <= createDto.AssignDate)
                {
                    return Task.FromResult((false, "La fecha de fin debe ser posterior a la fecha de asignación"));
                }

                return Task.FromResult((true, (string?)null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para crear asignación");
                return Task.FromResult((false, (string?)"Error interno durante la validación"));
            }
        }

        private Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(UpdateProjectAssignDto updateDto)
        {
            try
            {
                if (updateDto.AssignDate < DateTime.UtcNow.AddDays(-1))
                {
                    return Task.FromResult((false, "La fecha de asignación no puede ser anterior a ayer"));
                }

                if (updateDto.EndDate.HasValue && updateDto.EndDate <= updateDto.AssignDate)
                {
                    return Task.FromResult((false, "La fecha de fin debe ser posterior a la fecha de asignación"));
                }

                return Task.FromResult((true, (string?)null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para actualizar asignación");
                return Task.FromResult((false, (string?)"Error interno durante la validación"));
            }
        }

        private Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id)
        {
            try
            {
                // Aquí podrías agregar validaciones adicionales
                // Por ejemplo, verificar si hay registros de asistencia relacionados
                return Task.FromResult((true, (string?)null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando si se puede eliminar asignación con ID: {Id}", id);
                return Task.FromResult((false, (string?)"Error interno durante la validación"));
            }
        }

        private ProjectAssignListDto MapToProjectAssignListDto(ProjectsAssigns entity)
        {
            var daysAssigned = (DateTime.UtcNow - entity.AssignDate).Days;
            var isActive = entity.EndDate == null;
            var status = isActive ? "Activa" : "Finalizada";
            var isOverdue = entity.Project.EndDate.HasValue && entity.Project.EndDate < DateTime.UtcNow;

            return new ProjectAssignListDto(
                entity.Id,
                $"{entity.Employee.FirstName} {entity.Employee.LastName}",
                entity.Project.ProjectName,
                entity.AssignDate,
                entity.EndDate,
                isActive,
                status,
                daysAssigned,
                0m // TotalHoursWorked se calcularía con lógica adicional
            );
        }

        private ProjectAssignResponseDto MapToProjectAssignResponseDto(ProjectsAssigns entity)
        {
            var daysAssigned = (DateTime.UtcNow - entity.AssignDate).Days;
            var daysRemaining = entity.Project.EndDate.HasValue 
                ? (entity.Project.EndDate.Value - DateTime.UtcNow).Days 
                : (int?)null;
            var isActive = entity.EndDate == null;
            var status = isActive ? "Activa" : "Finalizada";
            var isOverdue = entity.Project.EndDate.HasValue && entity.Project.EndDate < DateTime.UtcNow;

            return new ProjectAssignResponseDto(
                entity.Id,
                entity.EmployeeId,
                $"{entity.Employee.FirstName} {entity.Employee.LastName}",
                "", // EmployeeEmail no existe en la entidad
                entity.ProjectId,
                entity.Project.ProjectName,
                "", // ProjectDescription no existe en la entidad
                entity.AssignDate,
                entity.EndDate,
                isActive,
                daysAssigned,
                daysRemaining,
                status,
                isOverdue,
                0m, // TotalHoursWorked se calcularía con lógica adicional
                0m, // AverageHoursPerDay se calcularía con lógica adicional
                entity.Project.State.StateName,
                entity.Project.EndDate,
                entity.Project.State.StateName != "Inactivo"
            );
        }

        private ProjectAssignDetailsDto MapToProjectAssignDetailsDto(ProjectsAssigns entity)
        {
            // Crear DTOs anidados
            var employeeInfo = new EmployeeInfoDto(
                entity.Employee.Id,
                $"{entity.Employee.FirstName} {entity.Employee.LastName}",
                "", // Email no existe en EmployeeInfo
                entity.Employee.PhoneNumber ?? "",
                entity.Employee.DocumentNumber,
                entity.Employee.DocumentType.DocumentName,
                entity.Employee.State.StateName,
                entity.Employee.CostPerHour,
                entity.Employee.RegisterDate,
                entity.Employee.State.StateName != "Inactivo"
            );

            var projectInfo = new ProjectInfoDto(
                entity.Project.Id,
                entity.Project.ProjectName,
                "", // Description no existe en Projects
                entity.Project.State.StateName,
                entity.Project.StartDate ?? DateTime.UtcNow,
                entity.Project.EndDate,
                0m, // Budget no existe en Projects
                0m, // ActualCost se calcularía
                entity.Project.State.StateName != "Inactivo",
                entity.Project.EndDate.HasValue,
                0, // TotalAssignedEmployees se calcularía
                0m // TotalProjectHours se calcularía
            );

            var assignmentStats = new AssignmentStatsDto(
                (DateTime.UtcNow - entity.AssignDate).Days,
                entity.Project.EndDate.HasValue ? (entity.Project.EndDate.Value - DateTime.UtcNow).Days : null,
                0m, // TotalHoursWorked se calcularía
                0m, // AverageHoursPerDay se calcularía
                0m, // RegularHours se calcularía
                0m, // OvertimeHours se calcularía
                0m, // EstimatedCost se calcularía
                0m, // ActualCost se calcularía
                entity.Project.EndDate.HasValue && entity.Project.EndDate < DateTime.UtcNow,
                "Normal", // PerformanceStatus se calcularía
                0m // CompletionPercentage se calcularía
            );

            return new ProjectAssignDetailsDto(
                entity.Id,
                entity.EmployeeId,
                $"{entity.Employee.FirstName} {entity.Employee.LastName}",
                "", // EmployeeEmail no existe
                entity.Employee.PhoneNumber ?? "", // EmployeePhone
                entity.Employee.DocumentNumber,
                entity.ProjectId,
                entity.Project.ProjectName,
                "", // ProjectDescription no existe
                entity.AssignDate,
                entity.EndDate,
                employeeInfo,
                projectInfo,
                assignmentStats,
                new List<AssignmentAssistanceDto>() // RecentAssistances se calcularía
            );
        }

        #endregion
    }
}
