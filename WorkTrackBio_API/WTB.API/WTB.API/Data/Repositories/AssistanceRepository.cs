using Microsoft.EntityFrameworkCore;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Assistance;
using WTB.API.Data.Repositories.Models;
using WTB.API.Data.Context;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de asistencia con métodos específicos del negocio
    /// </summary>
    public class AssistanceRepository : Repository<Assistance>, IAssistanceRepository
    {
        public AssistanceRepository(WTBDbContext context) : base(context)
        {
        }

        public async Task<Assistance?> GetActiveAssistanceAsync(int employeeId)
        {
            return await _dbSet
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && 
                                        a.CheckOut == null);
        }

        public async Task<bool> HasActiveAssistanceAsync(int employeeId)
        {
            return await _dbSet.AnyAsync(a => a.EmployeeId == employeeId && 
                                            a.CheckOut == null);
        }

        public async Task<IEnumerable<Assistance>> GetByEmployeeAndPeriodAsync(int employeeId, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.EmployeeId == employeeId && 
                           a.CheckIn >= fromDate && 
                           a.CheckIn <= toDate)
                .OrderByDescending(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByProjectAndPeriodAsync(int projectId, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.ProjectId == projectId && 
                           a.CheckIn >= fromDate && 
                           a.CheckIn <= toDate)
                .OrderByDescending(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Assistance> Items, int TotalCount)> GetFilteredAsync(
            AssistanceFilterDto filter,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(a => a.Employee.FirstName.ToLower().Contains(searchTerm) || 
                                       a.Employee.LastName.ToLower().Contains(searchTerm) ||
                                       a.Project.ProjectName.ToLower().Contains(searchTerm) ||
                                       (a.Notes != null && a.Notes.ToLower().Contains(searchTerm)));
            }

            if (filter.EmployeeId.HasValue)
                query = query.Where(a => a.EmployeeId == filter.EmployeeId.Value);

            if (filter.ProjectId.HasValue)
                query = query.Where(a => a.ProjectId == filter.ProjectId.Value);

            if (filter.CheckInFrom.HasValue)
                query = query.Where(a => a.CheckIn >= filter.CheckInFrom.Value);

            if (filter.CheckInTo.HasValue)
                query = query.Where(a => a.CheckIn <= filter.CheckInTo.Value);

            if (filter.CheckOutFrom.HasValue)
                query = query.Where(a => a.CheckOut >= filter.CheckOutFrom.Value);

            if (filter.CheckOutTo.HasValue)
                query = query.Where(a => a.CheckOut <= filter.CheckOutTo.Value);

            if (filter.DateFrom.HasValue)
                query = query.Where(a => a.CheckIn >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(a => a.CheckIn <= filter.DateTo.Value);

            if (!string.IsNullOrEmpty(filter.RegisterType))
                query = query.Where(a => a.RegisterType == filter.RegisterType);

            if (filter.IsActive.HasValue)
            {
                if (filter.IsActive.Value)
                    query = query.Where(a => a.CheckOut == null);
                else
                    query = query.Where(a => a.CheckOut != null);
            }

            if (filter.IsOvertime.HasValue)
            {
                if (filter.IsOvertime.Value)
                    query = query.Where(a => a.TotalHours > 8); // Más de 8 horas
                else
                    query = query.Where(a => a.TotalHours <= 8);
            }

            if (filter.HasNotes.HasValue)
            {
                if (filter.HasNotes.Value)
                    query = query.Where(a => !string.IsNullOrEmpty(a.Notes));
                else
                    query = query.Where(a => string.IsNullOrEmpty(a.Notes));
            }

            if (filter.MinHours.HasValue)
                query = query.Where(a => a.TotalHours >= filter.MinHours.Value);

            if (filter.MaxHours.HasValue)
                query = query.Where(a => a.TotalHours <= filter.MaxHours.Value);

            // Obtener total antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicar ordenamiento
            query = filter.SortBy?.ToLower() switch
            {
                "checkin" => filter.SortDescending ? 
                    query.OrderByDescending(a => a.CheckIn) :
                    query.OrderBy(a => a.CheckIn),
                "checkout" => filter.SortDescending ? 
                    query.OrderByDescending(a => a.CheckOut) :
                    query.OrderBy(a => a.CheckOut),
                "totalhours" => filter.SortDescending ? 
                    query.OrderByDescending(a => a.TotalHours) :
                    query.OrderBy(a => a.TotalHours),
                "employee" => filter.SortDescending ? 
                    query.OrderByDescending(a => a.Employee.FirstName) :
                    query.OrderBy(a => a.Employee.FirstName),
                "project" => filter.SortDescending ? 
                    query.OrderByDescending(a => a.Project.ProjectName) :
                    query.OrderBy(a => a.Project.ProjectName),
                _ => filter.SortDescending ? 
                    query.OrderByDescending(a => a.CheckIn) :
                    query.OrderBy(a => a.CheckIn)
            };

            // Aplicar paginación
            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<EmployeeWorkSummary>> GetEmployeeWorkSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            var query = from a in _dbSet
                       where a.CheckIn >= fromDate && a.CheckIn <= toDate && a.CheckOut != null
                       group a by new { a.EmployeeId, a.Employee.FirstName, a.Employee.LastName } into g
                       select new EmployeeWorkSummary
                       {
                           EmployeeId = g.Key.EmployeeId,
                           FullName = $"{g.Key.FirstName} {g.Key.LastName}",
                           TotalHours = g.Sum(a => a.TotalHours ?? 0),
                           TotalProjects = g.Select(a => a.ProjectId).Distinct().Count(),
                           TotalCost = g.Sum(a => (a.TotalHours ?? 0) * (a.Employee.CostPerHour ?? 0))
                       };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ProjectWorkSummary>> GetProjectWorkSummaryAsync(DateTime fromDate, DateTime toDate)
        {
            var query = from a in _dbSet
                       where a.CheckIn >= fromDate && a.CheckIn <= toDate && a.CheckOut != null
                       group a by new { a.ProjectId, a.Project.ProjectName } into g
                       select new ProjectWorkSummary
                       {
                           ProjectId = g.Key.ProjectId,
                           ProjectName = g.Key.ProjectName,
                           TotalEmployees = g.Select(a => a.EmployeeId).Distinct().Count(),
                           TotalHours = g.Sum(a => a.TotalHours ?? 0),
                           TotalCost = g.Sum(a => (a.TotalHours ?? 0) * (a.Employee.CostPerHour ?? 0)),
                           ProgressPercentage = 0, // Se puede calcular cuando se implemente
                           EstimatedEndDate = null // Se puede obtener de la entidad Project cuando se implemente
                       };

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetAssistancesNeedingCorrectionAsync()
        {
            // Asistencias que podrían necesitar corrección:
            // - Sin check-out después de 24 horas
            // - Con tiempo total excesivo
            // - Con inconsistencias en fechas
            var yesterday = DateTime.Today.AddDays(-1);
            
            return await _dbSet
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => (a.CheckOut == null && a.CheckIn < yesterday) || // Sin check-out por más de 24h
                           (a.TotalHours > 12) || // Más de 12 horas
                           (a.CheckOut < a.CheckIn)) // Check-out antes del check-in
                .OrderByDescending(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<AssistanceStatistics> GetAssistanceStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            var totalAssistances = await _dbSet
                .Where(a => a.CheckIn >= fromDate && a.CheckIn <= toDate)
                .CountAsync();

            var completedAssistances = await _dbSet
                .Where(a => a.CheckIn >= fromDate && a.CheckIn <= toDate && a.CheckOut != null)
                .CountAsync();

            var pendingAssistances = totalAssistances - completedAssistances;

            var totalHours = await _dbSet
                .Where(a => a.CheckIn >= fromDate && a.CheckIn <= toDate && a.CheckOut != null)
                .SumAsync(a => a.TotalHours ?? 0);

            var totalEmployees = await _dbSet
                .Where(a => a.CheckIn >= fromDate && a.CheckIn <= toDate)
                .Select(a => a.EmployeeId)
                .Distinct()
                .CountAsync();

            var totalProjects = await _dbSet
                .Where(a => a.CheckIn >= fromDate && a.CheckIn <= toDate)
                .Select(a => a.ProjectId)
                .Distinct()
                .CountAsync();

            var totalCost = await _dbSet
                .Where(a => a.CheckIn >= fromDate && a.CheckIn <= toDate && a.CheckOut != null)
                .SumAsync(a => (a.TotalHours ?? 0) * (a.Employee.CostPerHour ?? 0));

            var daysInPeriod = (toDate - fromDate).Days + 1;
            var averageHoursPerDay = daysInPeriod > 0 ? totalHours / daysInPeriod : 0;

            return new AssistanceStatistics
            {
                TotalAssistances = totalAssistances,
                CompletedAssistances = completedAssistances,
                PendingAssistances = pendingAssistances,
                TotalHours = totalHours,
                AverageHoursPerDay = averageHoursPerDay,
                TotalEmployees = totalEmployees,
                TotalProjects = totalProjects,
                TotalCost = totalCost
            };
        }

        public async Task<IEnumerable<TopWorkerSummary>> GetTopWorkersAsync(DateTime fromDate, DateTime toDate, int top = 10)
        {
            var query = from a in _dbSet
                       where a.CheckIn >= fromDate && a.CheckIn <= toDate && a.CheckOut != null
                       group a by new { a.EmployeeId, a.Employee.FirstName, a.Employee.LastName } into g
                       select new TopWorkerSummary
                       {
                           EmployeeId = g.Key.EmployeeId,
                           EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                           TotalHours = g.Sum(a => a.TotalHours ?? 0),
                           TotalDays = g.Count(),
                           AverageHoursPerDay = g.Average(a => a.TotalHours ?? 0),
                           TotalCost = g.Sum(a => (a.TotalHours ?? 0) * (a.Employee.CostPerHour ?? 0))
                       };

            return await query
                .OrderByDescending(w => w.TotalHours)
                .Take(top)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByWorkHoursRangeAsync(decimal minHours, decimal maxHours, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.CheckIn >= fromDate && 
                           a.CheckIn <= toDate && 
                           a.CheckOut != null &&
                           a.TotalHours >= minHours && 
                           a.TotalHours <= maxHours)
                .OrderByDescending(a => a.TotalHours)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetExcessiveWorkTimeAsync(DateTime fromDate, DateTime toDate, int maxHours = 12)
        {
            return await _dbSet
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.CheckIn >= fromDate && 
                           a.CheckIn <= toDate && 
                           a.CheckOut != null &&
                           a.TotalHours > maxHours)
                .OrderByDescending(a => a.TotalHours)
                .ToListAsync();
        }
    }
}
