using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Models.DTOs.ProjectMaintenance;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de mantenimientos de proyecto
    /// </summary>
    public class ProjectMaintenanceService : IProjectMaintenanceService
    {
        private readonly IProjectMaintenanceRepository _projectMaintenanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectMaintenanceService> _logger;

        public ProjectMaintenanceService(
            IProjectMaintenanceRepository projectMaintenanceRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProjectMaintenanceService> logger)
        {
            _projectMaintenanceRepository = projectMaintenanceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene mantenimientos de proyecto con filtros aplicados
        /// </summary>
        public async Task<PagedResponse<ProjectMaintenanceResponseDto>> GetFilteredAsync(ProjectMaintenanceFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos de proyecto filtrados");

                var pagedResult = await _projectMaintenanceRepository.GetFilteredAsync(filter);

                var responseDtos = pagedResult.Data?.Select(MapToResponseDto) ?? Enumerable.Empty<ProjectMaintenanceResponseDto>();

                return PagedResponse<ProjectMaintenanceResponseDto>.Create(responseDtos, pagedResult.Page, pagedResult.PageSize, pagedResult.TotalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos de proyecto filtrados");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un mantenimiento por ID
        /// </summary>
        public async Task<ProjectMaintenanceDetailsDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimiento con ID: {Id}", id);

                var maintenance = await _projectMaintenanceRepository.GetByIdWithIncludesAsync(id);
                if (maintenance == null)
                {
                    _logger.LogWarning("Mantenimiento con ID {Id} no encontrado", id);
                    return null;
                }

                var detailsDto = MapToDetailsDto(maintenance);
                _logger.LogInformation("Mantenimiento obtenido: {Description}", maintenance.MaintenanceDescription);

                return detailsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimiento con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo mantenimiento de proyecto
        /// </summary>
        public async Task<ProjectMaintenanceResponseDto> CreateAsync(CreateProjectMaintenanceDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando mantenimiento de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);

                // Validar datos de entrada
                var (isValid, errorMessage) = await ValidateForCreationAsync(createDto);
                if (!isValid)
                {
                    _logger.LogWarning("Validación fallida para crear mantenimiento: {Error}", errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Crear entidad
                var maintenance = new ProjectMaintenance
                {
                    IdProject = createDto.IdProject,
                    MaintenanceDate = createDto.MaintenanceDate ?? DateTime.Now,
                    MaintenanceDescription = createDto.MaintenanceDescription,
                    MadeById = createDto.MadeById,
                    MaintenanceCost = createDto.MaintenanceCost,
                    AdditionalInfo = createDto.AdditionalInfo,
                    StateId = createDto.StateId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Guardar en base de datos
                await _projectMaintenanceRepository.AddAsync(maintenance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mantenimiento creado exitosamente con ID: {Id}", maintenance.Id);

                return MapToResponseDto(maintenance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando mantenimiento de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un mantenimiento existente
        /// </summary>
        public async Task<ProjectMaintenanceResponseDto> UpdateAsync(int id, CreateProjectMaintenanceDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando mantenimiento con ID: {Id}", id);

                // Validar datos de entrada
                var (isValid, errorMessage) = await ValidateForUpdateAsync(id, updateDto);
                if (!isValid)
                {
                    _logger.LogWarning("Validación fallida para actualizar mantenimiento {Id}: {Error}", id, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Obtener mantenimiento existente
                var existingMaintenance = await _projectMaintenanceRepository.GetByIdAsync(id);
                if (existingMaintenance == null)
                {
                    _logger.LogWarning("Mantenimiento con ID {Id} no encontrado para actualizar", id);
                    throw new InvalidOperationException($"Mantenimiento con ID {id} no encontrado");
                }

                // Actualizar propiedades
                existingMaintenance.IdProject = updateDto.IdProject;
                existingMaintenance.MaintenanceDate = updateDto.MaintenanceDate ?? DateTime.Now;
                existingMaintenance.MaintenanceDescription = updateDto.MaintenanceDescription;
                existingMaintenance.MadeById = updateDto.MadeById;
                existingMaintenance.MaintenanceCost = updateDto.MaintenanceCost;
                existingMaintenance.AdditionalInfo = updateDto.AdditionalInfo;
                existingMaintenance.StateId = updateDto.StateId;
                existingMaintenance.ModifiedDate = DateTime.Now;

                // Actualizar en base de datos
                await _projectMaintenanceRepository.UpdateAsync(existingMaintenance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mantenimiento actualizado exitosamente: {Description}", existingMaintenance.MaintenanceDescription);

                return MapToResponseDto(existingMaintenance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando mantenimiento con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Elimina un mantenimiento (soft delete)
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando mantenimiento con ID: {Id}", id);

                // Validar si se puede eliminar
                var (canDelete, errorMessage) = await ValidateForDeletionAsync(id);
                if (!canDelete)
                {
                    _logger.LogWarning("No se puede eliminar mantenimiento {Id}: {Error}", id, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Obtener mantenimiento
                var maintenance = await _projectMaintenanceRepository.GetByIdAsync(id);
                if (maintenance == null)
                {
                    _logger.LogWarning("Mantenimiento con ID {Id} no encontrado para eliminar", id);
                    throw new InvalidOperationException($"Mantenimiento con ID {id} no encontrado");
                }

                // Soft delete - cambiar estado a eliminado
                maintenance.StateId = 3; // Estado eliminado (asumiendo que 3 es eliminado)
                maintenance.ModifiedDate = DateTime.Now;

                await _projectMaintenanceRepository.UpdateAsync(maintenance);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Mantenimiento eliminado exitosamente: {Description}", maintenance.MaintenanceDescription);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando mantenimiento con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por proyecto
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenanceListDto>> GetByProjectAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para proyecto ID: {ProjectId}", projectId);

                var maintenances = await _projectMaintenanceRepository.GetByProjectAsync(projectId);
                var listDtos = maintenances.Select(MapToListDto);

                _logger.LogInformation("Mantenimientos por proyecto obtenidos: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos para proyecto ID: {ProjectId}", projectId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por empleado
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenanceListDto>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para empleado ID: {EmployeeId}", employeeId);

                var maintenances = await _projectMaintenanceRepository.GetByEmployeeAsync(employeeId);
                var listDtos = maintenances.Select(MapToListDto);

                _logger.LogInformation("Mantenimientos por empleado obtenidos: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos para empleado ID: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por estado
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenanceListDto>> GetByStateAsync(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para estado ID: {StateId}", stateId);

                var maintenances = await _projectMaintenanceRepository.GetByStateAsync(stateId);
                var listDtos = maintenances.Select(MapToListDto);

                _logger.LogInformation("Mantenimientos por estado obtenidos: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos para estado ID: {StateId}", stateId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por rango de fechas
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenanceListDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos entre fechas: {FromDate} - {ToDate}", fromDate, toDate);

                var maintenances = await _projectMaintenanceRepository.GetByDateRangeAsync(fromDate, toDate);
                var listDtos = maintenances.Select(MapToListDto);

                _logger.LogInformation("Mantenimientos por rango de fechas obtenidos: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por rango de costo
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenanceListDto>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos entre costos: {MinCost} - {MaxCost}", minCost, maxCost);

                var maintenances = await _projectMaintenanceRepository.GetByCostRangeAsync(minCost, maxCost);
                var listDtos = maintenances.Select(MapToListDto);

                _logger.LogInformation("Mantenimientos por rango de costos obtenidos: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos por rango de costos: {MinCost} - {MaxCost}", minCost, maxCost);
                throw;
            }
        }

        /// <summary>
        /// Busca mantenimientos por término de búsqueda
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenanceListDto>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando mantenimientos con término: {SearchTerm}", searchTerm);

                var maintenances = await _projectMaintenanceRepository.SearchAsync(searchTerm);
                var listDtos = maintenances.Select(MapToListDto);

                _logger.LogInformation("Búsqueda completada: {Count} mantenimientos encontrados", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando mantenimientos con término: {SearchTerm}", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Obtiene estadísticas de mantenimientos
        /// </summary>
        public async Task<object> GetStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de mantenimientos");

                var statistics = await _projectMaintenanceRepository.GetStatisticsAsync();

                _logger.LogInformation("Estadísticas obtenidas exitosamente");

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de mantenimientos");
                throw;
            }
        }

        /// <summary>
        /// Valida los datos para crear un mantenimiento
        /// </summary>
        public Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateProjectMaintenanceDto createDto)
        {
            try
            {
                // Validaciones básicas ya están en el DTO con DataAnnotations
                // Aquí se pueden agregar validaciones de negocio adicionales

                // Verificar que el proyecto existe (se puede implementar después)
                // Verificar que el empleado existe (se puede implementar después)
                // Verificar que el estado existe (se puede implementar después)

                return Task.FromResult((true, (string?)null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para crear mantenimiento");
                return Task.FromResult((false, (string?)"Error interno durante la validación"));
            }
        }

        /// <summary>
        /// Valida los datos para actualizar un mantenimiento
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(int id, CreateProjectMaintenanceDto updateDto)
        {
            try
            {
                // Verificar que el mantenimiento existe
                var existingMaintenance = await _projectMaintenanceRepository.GetByIdAsync(id);
                if (existingMaintenance == null)
                {
                    return (false, $"Mantenimiento con ID {id} no encontrado");
                }

                // Validaciones básicas ya están en el DTO con DataAnnotations
                // Aquí se pueden agregar validaciones de negocio adicionales

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para actualizar mantenimiento con ID: {Id}", id);
                return (false, "Error interno durante la validación");
            }
        }

        /// <summary>
        /// Valida si se puede eliminar un mantenimiento
        /// </summary>
        public async Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id)
        {
            try
            {
                // Verificar que el mantenimiento existe
                var existingMaintenance = await _projectMaintenanceRepository.GetByIdAsync(id);
                if (existingMaintenance == null)
                {
                    return (false, $"Mantenimiento con ID {id} no encontrado");
                }

                // Por ahora permitimos eliminar cualquier mantenimiento
                // En el futuro se pueden agregar validaciones adicionales
                // como verificar si tiene registros relacionados

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando si se puede eliminar mantenimiento con ID: {Id}", id);
                return (false, "Error interno durante la validación");
            }
        }

        #region Private Methods

        /// <summary>
        /// Mapea entidad a DTO de respuesta
        /// </summary>
        private static ProjectMaintenanceResponseDto MapToResponseDto(ProjectMaintenance maintenance)
        {
            return new ProjectMaintenanceResponseDto(
                maintenance.Id,
                maintenance.IdProject,
                maintenance.Project?.ProjectName ?? "Proyecto no encontrado",
                maintenance.MaintenanceDate,
                maintenance.MaintenanceDescription,
                $"{maintenance.MadeBy?.FirstName} {maintenance.MadeBy?.LastName}".Trim(),
                maintenance.MaintenanceCost,
                maintenance.State?.StateName ?? "Estado no encontrado",
                maintenance.CreatedDate
            );
        }

        /// <summary>
        /// Mapea entidad a DTO de detalles
        /// </summary>
        private static ProjectMaintenanceDetailsDto MapToDetailsDto(ProjectMaintenance maintenance)
        {
            return new ProjectMaintenanceDetailsDto(
                maintenance.Id,
                maintenance.IdProject,
                maintenance.MaintenanceDate,
                maintenance.MaintenanceDescription,
                maintenance.MadeById,
                maintenance.MaintenanceCost,
                maintenance.AdditionalInfo,
                maintenance.StateId,
                maintenance.CreatedDate,
                maintenance.ModifiedDate,
                maintenance.Project?.ProjectName ?? "Proyecto no encontrado",
                maintenance.Project?.State?.StateName ?? "Estado del proyecto no encontrado",
                maintenance.MadeBy?.FirstName ?? "Nombre no encontrado",
                maintenance.MadeBy?.LastName ?? "Apellido no encontrado",
                maintenance.MadeBy?.DocumentNumber ?? "Documento no encontrado",
                maintenance.State?.StateName ?? "Estado no encontrado",
                maintenance.State?.StateType ?? "Tipo de estado no encontrado",
                maintenance.CreatedById,
                maintenance.ModifiedById,
                null, // CreatedBy - se puede implementar después
                null  // ModifiedBy - se puede implementar después
            );
        }

        /// <summary>
        /// Mapea entidad a DTO de lista
        /// </summary>
        private static ProjectMaintenanceListDto MapToListDto(ProjectMaintenance maintenance)
        {
            return new ProjectMaintenanceListDto(
                maintenance.Id,
                maintenance.Project?.ProjectName ?? "Proyecto no encontrado",
                maintenance.MaintenanceDate,
                maintenance.MaintenanceDescription,
                $"{maintenance.MadeBy?.FirstName} {maintenance.MadeBy?.LastName}".Trim(),
                maintenance.MaintenanceCost,
                maintenance.State?.StateName ?? "Estado no encontrado",
                maintenance.CreatedDate
            );
        }

        #endregion
    }
}
