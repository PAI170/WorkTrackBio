using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WTB.API.Data.Context;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectWarranty;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de garantías de proyecto
    /// </summary>
    public class ProjectWarrantyRepository : Repository<ProjectWarranty>, IProjectWarrantyRepository
    {
        private readonly ILogger<ProjectWarrantyRepository> _logger;

        public ProjectWarrantyRepository(WTBDbContext context, ILogger<ProjectWarrantyRepository> logger) 
            : base(context)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtiene garantías de proyecto con filtros aplicados
        /// </summary>
        public async Task<PagedResponse<ProjectWarranty>> GetFilteredAsync(ProjectWarrantyFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías de proyecto filtradas con: {Filter}", 
                    System.Text.Json.JsonSerializer.Serialize(filter));

                var query = _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .AsQueryable();

                // Aplicar filtros por proyecto
                if (filter.ProjectId.HasValue)
                {
                    query = query.Where(pw => pw.IdProject == filter.ProjectId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.ProjectName))
                {
                    var projectName = filter.ProjectName.ToLower();
                    query = query.Where(pw => pw.Project.ProjectName.ToLower().Contains(projectName));
                }

                // Aplicar filtros por empleado
                if (filter.EmployeeId.HasValue)
                {
                    query = query.Where(pw => pw.MadeById == filter.EmployeeId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.EmployeeName))
                {
                    var employeeName = filter.EmployeeName.ToLower();
                    query = query.Where(pw => 
                        pw.MadeBy.FirstName.ToLower().Contains(employeeName) ||
                        pw.MadeBy.LastName.ToLower().Contains(employeeName));
                }

                // Aplicar filtros por estado
                if (filter.StateId.HasValue)
                {
                    query = query.Where(pw => pw.StateId == filter.StateId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.StateName))
                {
                    var stateName = filter.StateName.ToLower();
                    query = query.Where(pw => pw.State.StateName.ToLower().Contains(stateName));
                }

                // Aplicar filtros por costo
                if (filter.MinCost.HasValue)
                {
                    query = query.Where(pw => pw.WarrantyCost >= filter.MinCost.Value);
                }

                if (filter.MaxCost.HasValue)
                {
                    query = query.Where(pw => pw.WarrantyCost <= filter.MaxCost.Value);
                }

                // Aplicar filtros por fecha
                if (filter.WarrantyDateFrom.HasValue)
                {
                    query = query.Where(pw => pw.WarrantyDate >= filter.WarrantyDateFrom.Value);
                }

                if (filter.WarrantyDateTo.HasValue)
                {
                    query = query.Where(pw => pw.WarrantyDate <= filter.WarrantyDateTo.Value);
                }

                if (filter.CreatedFrom.HasValue)
                {
                    query = query.Where(pw => pw.CreatedDate >= filter.CreatedFrom.Value);
                }

                if (filter.CreatedTo.HasValue)
                {
                    query = query.Where(pw => pw.CreatedDate <= filter.CreatedTo.Value);
                }

                // Aplicar filtros de texto
                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                {
                    var searchText = filter.SearchText.ToLower();
                    query = query.Where(pw => 
                        pw.WarrantyDescription.ToLower().Contains(searchText) ||
                        pw.Project.ProjectName.ToLower().Contains(searchText) ||
                        pw.MadeBy.FirstName.ToLower().Contains(searchText) ||
                        pw.MadeBy.LastName.ToLower().Contains(searchText) ||
                        pw.State.StateName.ToLower().Contains(searchText));
                }

                // Ordenar por fecha de garantía descendente
                query = query.OrderByDescending(pw => pw.WarrantyDate);

                // Contar total de registros
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var items = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                _logger.LogInformation("Garantías obtenidas: {Count} de {Total}", items.Count, totalCount);

                return PagedResponse<ProjectWarranty>.Create(items, filter.PageNumber, filter.PageSize, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantías de proyecto filtradas");
                throw;
            }
        }

        /// <summary>
        /// Obtiene una garantía por ID con todas sus relaciones
        /// </summary>
        public async Task<ProjectWarranty?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantía con ID: {Id}", id);

                var warranty = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .FirstOrDefaultAsync(pw => pw.Id == id);

                if (warranty == null)
                {
                    _logger.LogWarning("Garantía con ID {Id} no encontrada", id);
                }
                else
                {
                    _logger.LogInformation("Garantía obtenida: {Description}", warranty.WarrantyDescription);
                }

                return warranty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garantía con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene garantías por proyecto
        /// </summary>
        public async Task<IEnumerable<ProjectWarranty>> GetByProjectAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para proyecto ID: {ProjectId}", projectId);

                var warranties = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .Where(pw => pw.IdProject == projectId)
                    .OrderByDescending(pw => pw.WarrantyDate)
                    .ToListAsync();

                _logger.LogInformation("Garantías por proyecto obtenidas: {Count}", warranties.Count);

                return warranties;
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
        public async Task<IEnumerable<ProjectWarranty>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para empleado ID: {EmployeeId}", employeeId);

                var warranties = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .Where(pw => pw.MadeById == employeeId)
                    .OrderByDescending(pw => pw.WarrantyDate)
                    .ToListAsync();

                _logger.LogInformation("Garantías por empleado obtenidas: {Count}", warranties.Count);

                return warranties;
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
        public async Task<IEnumerable<ProjectWarranty>> GetByStateAsync(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías para estado ID: {StateId}", stateId);

                var warranties = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .Where(pw => pw.StateId == stateId)
                    .OrderByDescending(pw => pw.WarrantyDate)
                    .ToListAsync();

                _logger.LogInformation("Garantías por estado obtenidas: {Count}", warranties.Count);

                return warranties;
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
        public async Task<IEnumerable<ProjectWarranty>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías entre fechas: {FromDate} - {ToDate}", fromDate, toDate);

                var warranties = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .Where(pw => pw.WarrantyDate >= fromDate && pw.WarrantyDate <= toDate)
                    .OrderByDescending(pw => pw.WarrantyDate)
                    .ToListAsync();

                _logger.LogInformation("Garantías por rango de fechas obtenidas: {Count}", warranties.Count);

                return warranties;
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
        public async Task<IEnumerable<ProjectWarranty>> GetByCostRangeAsync(decimal minCost, decimal maxCost)
        {
            try
            {
                _logger.LogInformation("Obteniendo garantías entre costos: {MinCost} - {MaxCost}", minCost, maxCost);

                var warranties = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .Where(pw => pw.WarrantyCost >= minCost && pw.WarrantyCost <= maxCost)
                    .OrderByDescending(pw => pw.WarrantyDate)
                    .ToListAsync();

                _logger.LogInformation("Garantías por rango de costos obtenidas: {Count}", warranties.Count);

                return warranties;
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
        public async Task<IEnumerable<ProjectWarranty>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando garantías con término: {SearchTerm}", searchTerm);

                var warranties = await _context.ProjectWarranty
                    .Include(pw => pw.Project)
                    .Include(pw => pw.MadeBy)
                    .Include(pw => pw.State)
                    .Where(pw => 
                        pw.WarrantyDescription.ToLower().Contains(searchTerm.ToLower()) ||
                        pw.Project.ProjectName.ToLower().Contains(searchTerm.ToLower()) ||
                        pw.MadeBy.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                        pw.MadeBy.LastName.ToLower().Contains(searchTerm.ToLower()) ||
                        pw.State.StateName.ToLower().Contains(searchTerm.ToLower()))
                    .OrderByDescending(pw => pw.WarrantyDate)
                    .ToListAsync();

                _logger.LogInformation("Búsqueda completada: {Count} garantías encontradas", warranties.Count);

                return warranties;
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

                var totalWarranties = await _context.ProjectWarranty.CountAsync();
                var totalCost = await _context.ProjectWarranty
                    .Where(pw => pw.WarrantyCost.HasValue)
                    .SumAsync(pw => pw.WarrantyCost!.Value);
                var averageCost = totalWarranties > 0 ? totalCost / totalWarranties : 0;
                var thisMonthWarranties = await _context.ProjectWarranty
                    .CountAsync(pw => pw.WarrantyDate.Month == DateTime.Now.Month && 
                                     pw.WarrantyDate.Year == DateTime.Now.Year);

                var statistics = new
                {
                    TotalWarranties = totalWarranties,
                    TotalCost = totalCost,
                    AverageCost = averageCost,
                    ThisMonthWarranties = thisMonthWarranties,
                    LastUpdated = DateTime.Now
                };

                _logger.LogInformation("Estadísticas obtenidas: {Statistics}", 
                    System.Text.Json.JsonSerializer.Serialize(statistics));

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de garantías");
                throw;
            }
        }
    }
}
