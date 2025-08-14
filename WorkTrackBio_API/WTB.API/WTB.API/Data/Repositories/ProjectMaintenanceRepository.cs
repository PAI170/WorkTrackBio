using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WTB.API.Data.Context;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectMaintenance;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de mantenimientos de proyecto
    /// </summary>
    public class ProjectMaintenanceRepository : Repository<ProjectMaintenance>, IProjectMaintenanceRepository
    {
        private readonly ILogger<ProjectMaintenanceRepository> _logger;

        public ProjectMaintenanceRepository(WTBDbContext context, ILogger<ProjectMaintenanceRepository> logger) 
            : base(context)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtiene mantenimientos de proyecto con filtros aplicados
        /// </summary>
        public async Task<PagedResponse<ProjectMaintenance>> GetFilteredAsync(ProjectMaintenanceFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos de proyecto filtrados con: {Filter}", 
                    System.Text.Json.JsonSerializer.Serialize(filter));

                var query = _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .AsQueryable();

                // Aplicar filtros por proyecto
                if (filter.ProjectId.HasValue)
                {
                    query = query.Where(pm => pm.IdProject == filter.ProjectId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.ProjectName))
                {
                    var projectName = filter.ProjectName.ToLower();
                    query = query.Where(pm => pm.Project.ProjectName.ToLower().Contains(projectName));
                }

                // Aplicar filtros por empleado
                if (filter.EmployeeId.HasValue)
                {
                    query = query.Where(pm => pm.MadeById == filter.EmployeeId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.EmployeeName))
                {
                    var employeeName = filter.EmployeeName.ToLower();
                    query = query.Where(pm => 
                        pm.MadeBy.FirstName.ToLower().Contains(employeeName) ||
                        pm.MadeBy.LastName.ToLower().Contains(employeeName));
                }

                // Aplicar filtros por estado
                if (filter.StateId.HasValue)
                {
                    query = query.Where(pm => pm.StateId == filter.StateId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.StateName))
                {
                    var stateName = filter.StateName.ToLower();
                    query = query.Where(pm => pm.State.StateName.ToLower().Contains(stateName));
                }

                // Aplicar filtros por costo
                if (filter.MinCost.HasValue)
                {
                    query = query.Where(pm => pm.MaintenanceCost >= filter.MinCost.Value);
                }

                if (filter.MaxCost.HasValue)
                {
                    query = query.Where(pm => pm.MaintenanceCost <= filter.MaxCost.Value);
                }

                // Aplicar filtros por fecha
                if (filter.MaintenanceDateFrom.HasValue)
                {
                    query = query.Where(pm => pm.MaintenanceDate >= filter.MaintenanceDateFrom.Value);
                }

                if (filter.MaintenanceDateTo.HasValue)
                {
                    query = query.Where(pm => pm.MaintenanceDate <= filter.MaintenanceDateTo.Value);
                }

                if (filter.CreatedFrom.HasValue)
                {
                    query = query.Where(pm => pm.CreatedDate >= filter.CreatedFrom.Value);
                }

                if (filter.CreatedTo.HasValue)
                {
                    query = query.Where(pm => pm.CreatedDate <= filter.CreatedTo.Value);
                }

                // Aplicar filtros de texto
                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    var searchText = filter.SearchText.ToLower();
                    query = query.Where(pm => 
                        pm.MaintenanceDescription.ToLower().Contains(searchText) ||
                        pm.Project.ProjectName.ToLower().Contains(searchText) ||
                        pm.MadeBy.FirstName.ToLower().Contains(searchText) ||
                        pm.MadeBy.LastName.ToLower().Contains(searchText) ||
                        pm.State.StateName.ToLower().Contains(searchText));
                }

                // Ordenar por fecha de mantenimiento descendente
                query = query.OrderByDescending(pm => pm.MaintenanceDate);

                // Contar total de registros
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var items = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                _logger.LogInformation("Mantenimientos obtenidos: {Count} de {Total}", items.Count, totalCount);

                return PagedResponse<ProjectMaintenance>.Create(items, filter.PageNumber, filter.PageSize, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimientos de proyecto filtrados");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un mantenimiento por ID con todas sus relaciones
        /// </summary>
        public async Task<ProjectMaintenance?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimiento con ID: {Id}", id);

                var maintenance = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .FirstOrDefaultAsync(pm => pm.Id == id);

                if (maintenance == null)
                {
                    _logger.LogWarning("Mantenimiento con ID {Id} no encontrado", id);
                }
                else
                {
                    _logger.LogInformation("Mantenimiento obtenido: {Description}", maintenance.MaintenanceDescription);
                }

                return maintenance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo mantenimiento con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene mantenimientos por proyecto
        /// </summary>
        public async Task<IEnumerable<ProjectMaintenance>> GetByProjectAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para proyecto ID: {ProjectId}", projectId);

                var maintenances = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .Where(pm => pm.IdProject == projectId)
                    .OrderByDescending(pm => pm.MaintenanceDate)
                    .ToListAsync();

                _logger.LogInformation("Mantenimientos por proyecto obtenidos: {Count}", maintenances.Count);

                return maintenances;
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
        public async Task<IEnumerable<ProjectMaintenance>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para empleado ID: {EmployeeId}", employeeId);

                var maintenances = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .Where(pm => pm.MadeById == employeeId)
                    .OrderByDescending(pm => pm.MaintenanceDate)
                    .ToListAsync();

                _logger.LogInformation("Mantenimientos por empleado obtenidos: {Count}", maintenances.Count);

                return maintenances;
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
        public async Task<IEnumerable<ProjectMaintenance>> GetByStateAsync(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos para estado ID: {StateId}", stateId);

                var maintenances = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .Where(pm => pm.StateId == stateId)
                    .OrderByDescending(pm => pm.MaintenanceDate)
                    .ToListAsync();

                _logger.LogInformation("Mantenimientos por estado obtenidos: {Count}", maintenances.Count);

                return maintenances;
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
        public async Task<IEnumerable<ProjectMaintenance>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos entre fechas: {FromDate} - {ToDate}", fromDate, toDate);

                var maintenances = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .Where(pm => pm.MaintenanceDate >= fromDate && pm.MaintenanceDate <= toDate)
                    .OrderByDescending(pm => pm.MaintenanceDate)
                    .ToListAsync();

                _logger.LogInformation("Mantenimientos por rango de fechas obtenidos: {Count}", maintenances.Count);

                return maintenances;
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
        public async Task<IEnumerable<ProjectMaintenance>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            try
            {
                _logger.LogInformation("Obteniendo mantenimientos entre costos: {MinCost} - {MaxCost}", minCost, maxCost);

                var maintenances = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .Where(pm => pm.MaintenanceCost >= minCost && pm.MaintenanceCost <= maxCost)
                    .OrderByDescending(pm => pm.MaintenanceDate)
                    .ToListAsync();

                _logger.LogInformation("Mantenimientos por rango de costos obtenidos: {Count}", maintenances.Count);

                return maintenances;
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
        public async Task<IEnumerable<ProjectMaintenance>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando mantenimientos con término: {SearchTerm}", searchTerm);

                var maintenances = await _context.ProjectMaintenance
                    .Include(pm => pm.Project)
                    .Include(pm => pm.MadeBy)
                    .Include(pm => pm.State)
                    .Where(pm => 
                        pm.MaintenanceDescription.ToLower().Contains(searchTerm.ToLower()) ||
                        pm.Project.ProjectName.ToLower().Contains(searchTerm.ToLower()) ||
                        pm.MadeBy.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                        pm.MadeBy.LastName.ToLower().Contains(searchTerm.ToLower()) ||
                        pm.State.StateName.ToLower().Contains(searchTerm.ToLower()))
                    .OrderByDescending(pm => pm.MaintenanceDate)
                    .ToListAsync();

                _logger.LogInformation("Búsqueda completada: {Count} mantenimientos encontrados", maintenances.Count);

                return maintenances;
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

                var totalMaintenances = await _context.ProjectMaintenance.CountAsync();
                var totalCost = await _context.ProjectMaintenance
                    .Where(pm => pm.MaintenanceCost.HasValue)
                    .SumAsync(pm => pm.MaintenanceCost!.Value);
                var averageCost = totalMaintenances > 0 ? totalCost / totalMaintenances : 0;
                var thisMonthMaintenances = await _context.ProjectMaintenance
                    .CountAsync(pm => pm.MaintenanceDate.Month == DateTime.Now.Month && 
                                     pm.MaintenanceDate.Year == DateTime.Now.Year);

                var statistics = new
                {
                    TotalMaintenances = totalMaintenances,
                    TotalCost = totalCost,
                    AverageCost = averageCost,
                    ThisMonthMaintenances = thisMonthMaintenances,
                    LastUpdated = DateTime.Now
                };

                _logger.LogInformation("Estadísticas obtenidas: {Statistics}", 
                    System.Text.Json.JsonSerializer.Serialize(statistics));

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de mantenimientos");
                throw;
            }
        }
    }
}
