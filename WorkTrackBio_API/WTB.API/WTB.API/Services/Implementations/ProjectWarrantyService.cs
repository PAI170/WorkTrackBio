using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Models.DTOs.ProjectWarranty;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de garantías de proyecto
    /// </summary>
    public class ProjectWarrantyService : IProjectWarrantyService
    {
        private readonly IProjectWarrantyRepository _projectWarrantyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectWarrantyService> _logger;

        public ProjectWarrantyService(
            IProjectWarrantyRepository projectWarrantyRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProjectWarrantyService> logger)
        {
            _projectWarrantyRepository = projectWarrantyRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene garantías de proyecto con filtros aplicados
        /// </summary>
        public async Task<PagedResponse<ProjectWarrantyResponseDto>> GetFilteredAsync(ProjectWarrantyFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías de proyecto filtradas");

                var pagedResult = await _projectWarrantyRepository.GetFilteredAsync(filter);

                var responseDtos = pagedResult.Data?.Select(MapToResponseDto) ?? Enumerable.Empty<ProjectWarrantyResponseDto>();

                return PagedResponse<ProjectWarrantyResponseDto>.Create(responseDtos, pagedResult.Page, pagedResult.PageSize, pagedResult.TotalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías de proyecto filtradas");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una garantía por ID
        /// </summary>
        public async Task<ProjectWarrantyDetailsDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantía con ID: {Id}", id);

                var warranty = await _projectWarrantyRepository.GetByIdWithIncludesAsync(id);
                if (warranty == null)
                {
                    _logger.LogWarning("Garantía con ID {Id} no encontrada", id);
                    return null;
                }

                var detailsDto = MapToDetailsDto(warranty);
                _logger.LogInformation("Garantía obtenida: {Description}", warranty.WarrantyDescription);

                return detailsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantía con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea una nueva garantía de proyecto
        /// </summary>
        public async Task<ProjectWarrantyResponseDto> CreateAsync(CreateProjectWarrantyDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando garantía de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);

                // Validar datos de entrada
                var (isValid, errorMessage) = await ValidateForCreationAsync(createDto);
                if (!isValid)
                {
                    _logger.LogWarning("Validación fallida para crear garantía: {Error}", errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Crear entidad
                var warranty = new ProjectWarranty
                {
                    IdProject = createDto.IdProject,
                    WarrantyDate = createDto.WarrantyDate ?? DateTime.Now,
                    WarrantyDescription = createDto.WarrantyDescription,
                    MadeById = createDto.MadeById,
                    WarrantyCost = createDto.WarrantyCost,
                    StateId = createDto.StateId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Guardar en base de datos
                await _projectWarrantyRepository.AddAsync(warranty);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Garantía creada exitosamente con ID: {Id}", warranty.Id);

                return MapToResponseDto(warranty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando garantía de proyecto para proyecto ID: {ProjectId}", createDto.IdProject);
                throw;
            }
        }

        /// <summary>
        /// Actualiza una garantía existente
        /// </summary>
        public async Task<ProjectWarrantyResponseDto> UpdateAsync(int id, CreateProjectWarrantyDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando garantía con ID: {Id}", id);

                // Validar datos de entrada
                var (isValid, errorMessage) = await ValidateForUpdateAsync(id, updateDto);
                if (!isValid)
                {
                    _logger.LogWarning("Validación fallida para actualizar garantía {Id}: {Error}", id, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Obtener garantía existente
                var existingWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingWarranty == null)
                {
                    _logger.LogWarning("Garantía con ID {Id} no encontrada para actualizar", id);
                    throw new InvalidOperationException($"Garantía con ID {id} no encontrada");
                }

                // Actualizar propiedades
                existingWarranty.IdProject = updateDto.IdProject;
                existingWarranty.WarrantyDate = updateDto.WarrantyDate ?? DateTime.Now;
                existingWarranty.WarrantyDescription = updateDto.WarrantyDescription;
                existingWarranty.MadeById = updateDto.MadeById;
                existingWarranty.WarrantyCost = updateDto.WarrantyCost;
                existingWarranty.StateId = updateDto.StateId;
                existingWarranty.ModifiedDate = DateTime.Now;

                // Actualizar en base de datos
                await _projectWarrantyRepository.UpdateAsync(existingWarranty);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Garantía actualizada exitosamente: {Description}", existingWarranty.WarrantyDescription);

                return MapToResponseDto(existingWarranty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando garantía con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Elimina una garantía (soft delete)
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando garantía con ID: {Id}", id);

                // Validar si se puede eliminar
                var (canDelete, errorMessage) = await ValidateForDeletionAsync(id);
                if (!canDelete)
                {
                    _logger.LogWarning("No se puede eliminar garantía {Id}: {Error}", id, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Obtener garantía
                var warranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (warranty == null)
                {
                    _logger.LogWarning("Garantía con ID {Id} no encontrada para eliminar", id);
                    throw new InvalidOperationException($"Garantía con ID {id} no encontrada");
                }

                // Soft delete - cambiar estado a eliminado
                warranty.StateId = 3; // Estado eliminado (asumiendo que 3 es eliminado)
                warranty.ModifiedDate = DateTime.Now;

                await _projectWarrantyRepository.UpdateAsync(warranty);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Garantía eliminada exitosamente: {Description}", warranty.WarrantyDescription);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando garantía con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene garantías por proyecto
        /// </summary>
        public async Task<IEnumerable<ProjectWarrantyListDto>> GetByProjectAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para proyecto ID: {ProjectId}", projectId);

                var warranties = await _projectWarrantyRepository.GetByProjectAsync(projectId);
                var listDtos = warranties.Select(MapToListDto);

                _logger.LogInformation("Garantías por proyecto obtenidas: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías para proyecto ID: {ProjectId}", projectId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene garantías por empleado
        /// </summary>
        public async Task<IEnumerable<ProjectWarrantyListDto>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para empleado ID: {EmployeeId}", employeeId);

                var warranties = await _projectWarrantyRepository.GetByEmployeeAsync(employeeId);
                var listDtos = warranties.Select(MapToListDto);

                _logger.LogInformation("Garantías por empleado obtenidas: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías para empleado ID: {EmployeeId}", employeeId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene garantías por estado
        /// </summary>
        public async Task<IEnumerable<ProjectWarrantyListDto>> GetByStateAsync(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para estado ID: {StateId}", stateId);

                var warranties = await _projectWarrantyRepository.GetByStateAsync(stateId);
                var listDtos = warranties.Select(MapToListDto);

                _logger.LogInformation("Garantías por estado obtenidas: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías para estado ID: {StateId}", stateId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene garantías por rango de fechas
        /// </summary>
        public async Task<IEnumerable<ProjectWarrantyListDto>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías entre fechas: {FromDate} - {ToDate}", fromDate, toDate);

                var warranties = await _projectWarrantyRepository.GetByDateRangeAsync(fromDate, toDate);
                var listDtos = warranties.Select(MapToListDto);

                _logger.LogInformation("Garantías por rango de fechas obtenidas: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        /// <summary>
        /// Obtiene garantías por rango de costo
        /// </summary>
        public async Task<IEnumerable<ProjectWarrantyListDto>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías entre costos: {MinCost} - {MaxCost}", minCost, maxCost);

                var warranties = await _projectWarrantyRepository.GetByCostRangeAsync(minCost, maxCost);
                var listDtos = warranties.Select(MapToListDto);

                _logger.LogInformation("Garantías por rango de costos obtenidas: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías por rango de costos: {MinCost} - {MaxCost}", minCost, maxCost);
                throw;
            }
        }

        /// <summary>
        /// Busca garantías por término de búsqueda
        /// </summary>
        public async Task<IEnumerable<ProjectWarrantyListDto>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando garantías con término: {SearchTerm}", searchTerm);

                var warranties = await _projectWarrantyRepository.SearchAsync(searchTerm);
                var listDtos = warranties.Select(MapToListDto);

                _logger.LogInformation("Búsqueda completada: {Count} garantías encontradas", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando garantías con término: {SearchTerm}", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Obtiene estadísticas de garantías
        /// </summary>
        public async Task<object> GetStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de garantías");

                var statistics = await _projectWarrantyRepository.GetStatisticsAsync();

                _logger.LogInformation("Estadísticas obtenidas exitosamente");

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de garantías");
                throw;
            }
        }

        /// <summary>
        /// Valida los datos para crear una garantía
        /// </summary>
        public Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateProjectWarrantyDto createDto)
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
                _logger.LogError(ex, "Error validando datos para crear garantía");
                return Task.FromResult((false, (string?)"Error interno durante la validación"));
            }
        }

        /// <summary>
        /// Valida los datos para actualizar una garantía
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(int id, CreateProjectWarrantyDto updateDto)
        {
            try
            {
                // Verificar que la garantía existe
                var existingWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingWarranty == null)
                {
                    return (false, $"Garantía con ID {id} no encontrada");
                }

                // Validaciones básicas ya están en el DTO con DataAnnotations
                // Aquí se pueden agregar validaciones de negocio adicionales

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para actualizar garantía con ID: {Id}", id);
                return (false, "Error interno durante la validación");
            }
        }

        /// <summary>
        /// Valida si se puede eliminar una garantía
        /// </summary>
        public async Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id)
        {
            try
            {
                // Verificar que la garantía existe
                var existingWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingWarranty == null)
                {
                    return (false, $"Garantía con ID {id} no encontrada");
                }

                // Por ahora permitimos eliminar cualquier garantía
                // En el futuro se pueden agregar validaciones adicionales
                // como verificar si tiene registros relacionados

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando si se puede eliminar garantía con ID: {Id}", id);
                return (false, "Error interno durante la validación");
            }
        }

        #region Private Methods

        /// <summary>
        /// Mapea entidad a DTO de respuesta
        /// </summary>
        private static ProjectWarrantyResponseDto MapToResponseDto(ProjectWarranty warranty)
        {
            return new ProjectWarrantyResponseDto(
                warranty.Id,
                warranty.IdProject,
                warranty.Project?.ProjectName ?? "Proyecto no encontrado",
                warranty.WarrantyDate,
                warranty.WarrantyDescription,
                $"{warranty.MadeBy?.FirstName} {warranty.MadeBy?.LastName}".Trim(),
                warranty.WarrantyCost,
                warranty.State?.StateName ?? "Estado no encontrado",
                warranty.CreatedDate
            );
        }

        /// <summary>
        /// Mapea entidad a DTO de detalles
        /// </summary>
        private static ProjectWarrantyDetailsDto MapToDetailsDto(ProjectWarranty warranty)
        {
            return new ProjectWarrantyDetailsDto(
                warranty.Id,
                warranty.IdProject,
                warranty.WarrantyDate,
                warranty.WarrantyDescription,
                warranty.MadeById,
                warranty.WarrantyCost,
                warranty.StateId,
                warranty.CreatedDate,
                warranty.ModifiedDate,
                warranty.Project?.ProjectName ?? "Proyecto no encontrado",
                warranty.Project?.State?.StateName ?? "Estado del proyecto no encontrado",
                warranty.MadeBy?.FirstName ?? "Nombre no encontrado",
                warranty.MadeBy?.LastName ?? "Apellido no encontrado",
                warranty.MadeBy?.DocumentNumber ?? "Documento no encontrado",
                warranty.State?.StateName ?? "Estado no encontrado",
                warranty.State?.StateType ?? "Tipo de estado no encontrado",
                warranty.CreatedById,
                warranty.ModifiedById,
                null, // CreatedBy - se puede implementar después
                null  // ModifiedBy - se puede implementar después
            );
        }

        /// <summary>
        /// Mapea entidad a DTO de lista
        /// </summary>
        private static ProjectWarrantyListDto MapToListDto(ProjectWarranty warranty)
        {
            return new ProjectWarrantyListDto(
                warranty.Id,
                warranty.Project?.ProjectName ?? "Proyecto no encontrado",
                warranty.WarrantyDate,
                warranty.WarrantyDescription,
                $"{warranty.MadeBy?.FirstName} {warranty.MadeBy?.LastName}".Trim(),
                warranty.WarrantyCost,
                warranty.State?.StateName ?? "Estado no encontrado",
                warranty.CreatedDate
            );
        }

        #endregion
    }
}
