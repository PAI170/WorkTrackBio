using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WTB.API.Data.Context;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.ProjectAssign;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de asignaciones de proyectos
    /// </summary>
    public class ProjectAssignRepository : Repository<ProjectsAssigns>, IProjectAssignRepository
    {
        private readonly ILogger<ProjectAssignRepository> _logger;

        public ProjectAssignRepository(WTBDbContext context, ILogger<ProjectAssignRepository> logger) 
            : base(context)
        {
            _logger = logger;
        }

        public async Task<PagedResponse<ProjectsAssigns>> GetFilteredAsync(ProjectAssignFilterDto filter)
        {
            try
            {
                var query = _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.DocumentType)
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(pa => 
                        pa.Employee.FirstName.ToLower().Contains(searchTerm) ||
                        pa.Employee.LastName.ToLower().Contains(searchTerm) ||
                        pa.Employee.DocumentNumber.ToLower().Contains(searchTerm) ||
                        pa.Project.ProjectName.ToLower().Contains(searchTerm));
                }

                if (filter.EmployeeId.HasValue)
                {
                    query = query.Where(pa => pa.EmployeeId == filter.EmployeeId.Value);
                }

                if (!string.IsNullOrEmpty(filter.EmployeeName))
                {
                    var employeeName = filter.EmployeeName.ToLower();
                    query = query.Where(pa => 
                        pa.Employee.FirstName.ToLower().Contains(employeeName) ||
                        pa.Employee.LastName.ToLower().Contains(employeeName));
                }

                if (filter.ProjectId.HasValue)
                {
                    query = query.Where(pa => pa.ProjectId == filter.ProjectId.Value);
                }

                if (!string.IsNullOrEmpty(filter.ProjectName))
                {
                    var projectName = filter.ProjectName.ToLower();
                    query = query.Where(pa => pa.Project.ProjectName.ToLower().Contains(projectName));
                }

                if (filter.IsActive.HasValue)
                {
                    if (filter.IsActive.Value)
                    {
                        query = query.Where(pa => pa.EndDate == null);
                    }
                    else
                    {
                        query = query.Where(pa => pa.EndDate != null);
                    }
                }

                if (filter.AssignedFrom.HasValue)
                {
                    query = query.Where(pa => pa.AssignDate >= filter.AssignedFrom.Value);
                }

                if (filter.AssignedTo.HasValue)
                {
                    query = query.Where(pa => pa.AssignDate <= filter.AssignedTo.Value);
                }

                if (filter.EndDateFrom.HasValue)
                {
                    query = query.Where(pa => pa.EndDate >= filter.EndDateFrom.Value);
                }

                if (filter.EndDateTo.HasValue)
                {
                    query = query.Where(pa => pa.EndDate <= filter.EndDateTo.Value);
                }

                if (filter.IsOverdue.HasValue)
                {
                    if (filter.IsOverdue.Value)
                    {
                        query = query.Where(pa => pa.Project.EndDate.HasValue && pa.Project.EndDate < DateTime.UtcNow);
                    }
                    else
                    {
                        query = query.Where(pa => !pa.Project.EndDate.HasValue || pa.Project.EndDate >= DateTime.UtcNow);
                    }
                }

                if (!string.IsNullOrEmpty(filter.ProjectState))
                {
                    query = query.Where(pa => pa.Project.State.StateName == filter.ProjectState);
                }

                if (!string.IsNullOrEmpty(filter.EmployeeState))
                {
                    query = query.Where(pa => pa.Employee.State.StateName == filter.EmployeeState);
                }

                // Aplicar ordenamiento
                query = filter.SortBy?.ToLower() switch
                {
                    "employeename" => filter.SortDescending 
                        ? query.OrderByDescending(pa => pa.Employee.FirstName)
                        : query.OrderBy(pa => pa.Employee.FirstName),
                    "projectname" => filter.SortDescending 
                        ? query.OrderByDescending(pa => pa.Project.ProjectName)
                        : query.OrderBy(pa => pa.Project.ProjectName),
                    "assigndate" => filter.SortDescending 
                        ? query.OrderByDescending(pa => pa.AssignDate)
                        : query.OrderBy(pa => pa.AssignDate),
                    "enddate" => filter.SortDescending 
                        ? query.OrderByDescending(pa => pa.EndDate)
                        : query.OrderBy(pa => pa.EndDate),
                    _ => filter.SortDescending 
                        ? query.OrderByDescending(pa => pa.AssignDate)
                        : query.OrderBy(pa => pa.AssignDate)
                };

                var totalRecords = await query.CountAsync();
                var data = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                return PagedResponse<ProjectsAssigns>.Create(data, totalRecords, filter.Page, filter.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones filtradas");
                throw;
            }
        }

        public async Task<ProjectsAssigns?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.DocumentType)
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .FirstOrDefaultAsync(pa => pa.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignación con ID: {Id} e includes", id);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectsAssigns>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .Where(pa => pa.EmployeeId == employeeId)
                    .OrderByDescending(pa => pa.AssignDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones del empleado con ID: {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectsAssigns>> GetByProjectAsync(int projectId)
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Where(pa => pa.ProjectId == projectId)
                    .OrderByDescending(pa => pa.AssignDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones del proyecto con ID: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectsAssigns>> GetActiveAsync()
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .Where(pa => pa.EndDate == null)
                    .OrderByDescending(pa => pa.AssignDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones activas");
                throw;
            }
        }

        public async Task<IEnumerable<ProjectsAssigns>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .Where(pa => pa.AssignDate >= fromDate && pa.AssignDate <= toDate)
                    .OrderByDescending(pa => pa.AssignDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectsAssigns>> GetByProjectStateAsync(string projectState)
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .Where(pa => pa.Project.State.StateName == projectState)
                    .OrderByDescending(pa => pa.AssignDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo asignaciones por estado del proyecto: {ProjectState}", projectState);
                throw;
            }
        }

        public async Task<IEnumerable<ProjectsAssigns>> GetByEmployeeStateAsync(string employeeState)
        {
            try
            {
                return await _context.ProjectsAssigns
                    .Include(pa => pa.Employee)
                        .ThenInclude(e => e.State)
                    .Include(pa => pa.Project)
                        .ThenInclude(p => p.State)
                    .Where(pa => pa.Employee.State.StateName == employeeState)
                    .OrderByDescending(pa => pa.AssignDate)
                    .ToListAsync();
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
                var query = _context.ProjectsAssigns
                    .Where(pa => pa.EmployeeId == employeeId && pa.ProjectId == projectId);

                if (fromDate.HasValue)
                {
                    query = query.Where(pa => pa.AssignDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(pa => pa.AssignDate <= toDate.Value);
                }

                return await query.AnyAsync();
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
                var totalAssignments = await _context.ProjectsAssigns.CountAsync();
                var activeAssignments = await _context.ProjectsAssigns.CountAsync(pa => pa.EndDate == null);
                var completedAssignments = await _context.ProjectsAssigns.CountAsync(pa => pa.EndDate != null);
                var overdueAssignments = await _context.ProjectsAssigns.CountAsync(pa => 
                    pa.Project.EndDate.HasValue && pa.Project.EndDate < DateTime.UtcNow);

                var totalEmployees = await _context.EmployeeInfo.CountAsync();
                var totalProjects = await _context.Projects.CountAsync();

                // Calcular horas trabajadas (esto requeriría lógica adicional con Assistance)
                var totalHoursWorked = 0m;
                var averageHoursPerAssignment = totalAssignments > 0 ? totalHoursWorked / totalAssignments : 0m;

                return new ProjectAssignStatsDto(
                    totalAssignments,
                    activeAssignments,
                    completedAssignments,
                    overdueAssignments,
                    totalHoursWorked,
                    averageHoursPerAssignment,
                    totalEmployees,
                    totalProjects
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estadísticas de asignaciones");
                throw;
            }
        }
    }
}
