using Microsoft.EntityFrameworkCore;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Project;
using WTB.API.Data.Repositories.Models;
using WTB.API.Data.Context;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de proyectos con métodos específicos del negocio
    /// </summary>
    public class ProjectRepository : Repository<Projects>, IProjectRepository
    {
        public ProjectRepository(WTBDbContext context) : base(context)
        {
        }

        public async Task<Projects?> GetByNameAsync(string projectName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.ProjectName == projectName);
        }

        public async Task<bool> ExistsByNameAsync(string projectName)
        {
            return await _dbSet.AnyAsync(p => p.ProjectName == projectName);
        }

        public async Task<IEnumerable<Projects>> GetByStateAsync(int stateId)
        {
            return await _dbSet
                .Where(p => p.StateId == stateId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projects>> GetActiveProjectsAsync()
        {
            // Asumiendo que el estado activo tiene ID = 1 (ajustar según tu configuración)
            return await _dbSet
                .Where(p => p.StateId == 1)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projects>> GetByClientAsync(string clientName)
        {
            // La entidad Projects no tiene ClientName, se puede implementar cuando se agregue
            // Por ahora retornamos una lista vacía
            return await Task.FromResult(Enumerable.Empty<Projects>());
        }

        public async Task<IEnumerable<Projects>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Where(p => p.StartDate >= fromDate && p.StartDate <= toDate)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Projects> Items, int TotalCount)> GetFilteredAsync(
            ProjectFilterDto filter,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(p => p.ProjectName.ToLower().Contains(searchTerm));
            }

            if (filter.StateId.HasValue)
                query = query.Where(p => p.StateId == filter.StateId.Value);

            if (filter.StartDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= filter.StartDateFrom.Value);

            if (filter.StartDateTo.HasValue)
                query = query.Where(p => p.StartDate <= filter.StartDateTo.Value);

            if (filter.EndDateFrom.HasValue)
                query = query.Where(p => p.EndDate >= filter.EndDateFrom.Value);

            if (filter.EndDateTo.HasValue)
                query = query.Where(p => p.EndDate <= filter.EndDateTo.Value);

            if (filter.IsActive.HasValue)
            {
                var activeStateId = filter.IsActive.Value ? 1 : 2; // Ajustar según configuración
                query = query.Where(p => p.StateId == activeStateId);
            }

            if (filter.IsCompleted.HasValue)
            {
                var completedStateId = filter.IsCompleted.Value ? 2 : 1; // Ajustar según configuración
                query = query.Where(p => p.StateId == completedStateId);
            }

            if (filter.IsOverdue.HasValue)
            {
                if (filter.IsOverdue.Value)
                    query = query.Where(p => p.EndDate.HasValue && p.EndDate < DateTime.Today);
                else
                    query = query.Where(p => !p.EndDate.HasValue || p.EndDate >= DateTime.Today);
            }

            if (filter.EmployeeId.HasValue)
            {
                // Filtrar por empleado asignado al proyecto
                query = query.Where(p => p.ProjectsAssigns.Any(pa => pa.EmployeeId == filter.EmployeeId.Value));
            }

            if (filter.MinHoursWorked.HasValue)
            {
                // Filtrar por horas trabajadas mínimas
                query = query.Where(p => p.Assistances.Sum(a => a.TotalHours) >= filter.MinHoursWorked.Value);
            }

            if (filter.MaxHoursWorked.HasValue)
            {
                // Filtrar por horas trabajadas máximas
                query = query.Where(p => p.Assistances.Sum(a => a.TotalHours) <= filter.MaxHoursWorked.Value);
            }

            // Obtener total antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicar ordenamiento
            query = filter.SortBy?.ToLower() switch
            {
                "projectname" => filter.SortDescending ? 
                    query.OrderByDescending(p => p.ProjectName) :
                    query.OrderBy(p => p.ProjectName),
                "startdate" => filter.SortDescending ? 
                    query.OrderByDescending(p => p.StartDate) :
                    query.OrderBy(p => p.StartDate),
                "enddate" => filter.SortDescending ? 
                    query.OrderByDescending(p => p.EndDate) :
                    query.OrderBy(p => p.EndDate),
                "state" => filter.SortDescending ? 
                    query.OrderByDescending(p => p.StateId) :
                    query.OrderBy(p => p.StateId),
                _ => filter.SortDescending ? 
                    query.OrderByDescending(p => p.StartDate) :
                    query.OrderBy(p => p.StartDate)
            };

            // Aplicar paginación
            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<ProjectWorkSummary>> GetTopWorkingProjectsAsync(DateTime fromDate, DateTime toDate, int top = 10)
        {
            // Esta consulta requeriría un join con la tabla Assistance
            // Por ahora retornamos una lista vacía hasta implementar la lógica completa
            return await Task.FromResult(Enumerable.Empty<ProjectWorkSummary>());
        }

        public async Task<ProjectStatistics> GetProjectStatisticsAsync()
        {
            var totalProjects = await _dbSet.CountAsync();
            var activeProjects = await _dbSet.CountAsync(p => p.StateId == 1); // Ajustar según configuración
            var completedProjects = await _dbSet.CountAsync(p => p.StateId == 2); // Ajustar según configuración
            var pendingProjects = totalProjects - activeProjects - completedProjects;
            
            // La entidad Projects no tiene Budget ni SpentAmount
            var totalBudget = 0m;
            var totalSpent = 0m;

            var averageProjectDuration = (decimal)await _dbSet
                .Where(p => p.StartDate.HasValue && p.EndDate.HasValue)
                .Select(p => (p.EndDate!.Value - p.StartDate!.Value).Days)
                .AverageAsync();

            var totalEmployees = await _dbSet
                .SelectMany(p => p.ProjectsAssigns)
                .Select(pa => pa.EmployeeId)
                .Distinct()
                .CountAsync();

            return new ProjectStatistics
            {
                TotalProjects = totalProjects,
                ActiveProjects = activeProjects,
                CompletedProjects = completedProjects,
                PendingProjects = pendingProjects,
                TotalBudget = totalBudget,
                TotalSpent = totalSpent,
                AverageProjectDuration = averageProjectDuration,
                TotalEmployees = totalEmployees
            };
        }

        public async Task<IEnumerable<Projects>> GetProjectsNeedingAttentionAsync(int daysThreshold = 7)
        {
            var thresholdDate = DateTime.Today.AddDays(daysThreshold);
            
            return await _dbSet
                .Where(p => p.EndDate.HasValue && p.EndDate <= thresholdDate && p.StateId == 1)
                .OrderBy(p => p.EndDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Projects>> GetByBudgetRangeAsync(decimal minBudget, decimal maxBudget)
        {
            // La entidad Projects no tiene Budget, se puede implementar cuando se agregue
            // Por ahora retornamos una lista vacía
            return await Task.FromResult(Enumerable.Empty<Projects>());
        }

        public async Task<IEnumerable<Projects>> SearchProjectsAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _dbSet
                .Where(p => p.ProjectName.ToLower().Contains(term))
                .ToListAsync();
        }

        public async Task<IEnumerable<Projects>> GetProjectsByEmployeeAsync(int employeeId)
        {
            // Esta implementación requeriría lógica adicional para obtener proyectos por empleado
            // Por ahora retornamos una lista vacía
            return await Task.FromResult(Enumerable.Empty<Projects>());
        }
    }
}
