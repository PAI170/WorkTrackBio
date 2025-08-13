using Microsoft.EntityFrameworkCore;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.Employee;
using WTB.API.Data.Repositories.Models;
using WTB.API.Data.Context;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de empleados con métodos específicos del negocio
    /// </summary>
    public class EmployeeRepository : Repository<EmployeeInfo>, IEmployeeRepository
    {
        public EmployeeRepository(WTBDbContext context) : base(context)
        {
        }

        public async Task<EmployeeInfo?> GetByDocumentNumberAsync(string documentNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber);
        }

        public async Task<EmployeeInfo?> GetByDocumentNumberWithIncludesAsync(string documentNumber)
        {
            return await _dbSet
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .Include(e => e.FingerPrints)
                .FirstOrDefaultAsync(e => e.DocumentNumber == documentNumber);
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber)
        {
            return await _dbSet.AnyAsync(e => e.DocumentNumber == documentNumber);
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByDocumentTypeAsync(int documentTypeId)
        {
            return await _dbSet
                .Where(e => e.DocumentTypeId == documentTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByStateAsync(int stateId)
        {
            return await _dbSet
                .Where(e => e.StateId == stateId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> GetActiveEmployeesAsync()
        {
            // Asumiendo que el estado activo tiene ID = 1 (ajustar según tu configuración)
            return await _dbSet
                .Where(e => e.StateId == 1)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> GetEmployeesWithFingerPrintAsync()
        {
            return await _dbSet
                .Include(e => e.FingerPrints)
                .Where(e => e.FingerPrints != null && e.FingerPrints.Any())
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByAgeRangeAsync(int minAge, int maxAge)
        {
            var today = DateTime.Today;
            var minDate = today.AddYears(-maxAge);
            var maxDate = today.AddYears(-minAge);

            return await _dbSet
                .Where(e => e.Birthday >= minDate && e.Birthday <= maxDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByCostPerHourRangeAsync(decimal minCost, decimal maxCost)
        {
            return await _dbSet
                .Where(e => e.CostPerHour >= minCost && e.CostPerHour <= maxCost)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> SearchByNameAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _dbSet
                .Where(e => e.FirstName.ToLower().Contains(term) || 
                           e.LastName.ToLower().Contains(term))
                .ToListAsync();
        }

        public async Task<(IEnumerable<EmployeeInfo> Items, int TotalCount)> GetFilteredAsync(
            EmployeeFilterDto filter,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _dbSet.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(e => e.FirstName.ToLower().Contains(searchTerm) || 
                                       e.LastName.ToLower().Contains(searchTerm) ||
                                       e.DocumentNumber.Contains(searchTerm));
            }

            if (filter.DocumentTypeId.HasValue)
                query = query.Where(e => e.DocumentTypeId == filter.DocumentTypeId.Value);

            if (filter.StateId.HasValue)
                query = query.Where(e => e.StateId == filter.StateId.Value);

            if (filter.RegisterDateFrom.HasValue)
                query = query.Where(e => e.RegisterDate >= filter.RegisterDateFrom.Value);

            if (filter.RegisterDateTo.HasValue)
                query = query.Where(e => e.RegisterDate <= filter.RegisterDateTo.Value);

            if (filter.BirthdayFrom.HasValue)
                query = query.Where(e => e.Birthday >= filter.BirthdayFrom.Value);

            if (filter.BirthdayTo.HasValue)
                query = query.Where(e => e.Birthday <= filter.BirthdayTo.Value);

            if (filter.CostPerHourMin.HasValue)
                query = query.Where(e => e.CostPerHour >= filter.CostPerHourMin.Value);

            if (filter.CostPerHourMax.HasValue)
                query = query.Where(e => e.CostPerHour <= filter.CostPerHourMax.Value);

            if (filter.HasFingerPrint.HasValue)
            {
                if (filter.HasFingerPrint.Value)
                    query = query.Where(e => e.FingerPrints != null && e.FingerPrints.Any());
                else
                    query = query.Where(e => e.FingerPrints == null || !e.FingerPrints.Any());
            }

            if (filter.IsActive.HasValue)
            {
                var activeStateId = filter.IsActive.Value ? 1 : 2; // Ajustar según configuración
                query = query.Where(e => e.StateId == activeStateId);
            }

            // Obtener total antes de paginar
            var totalCount = await query.CountAsync();

            // Aplicar ordenamiento
            query = filter.SortBy?.ToLower() switch
            {
                "firstname" => filter.SortDescending ? 
                    query.OrderByDescending(e => e.FirstName) :
                    query.OrderBy(e => e.FirstName),
                "lastname" => filter.SortDescending ? 
                    query.OrderByDescending(e => e.LastName) :
                    query.OrderBy(e => e.LastName),
                "documentnumber" => filter.SortDescending ? 
                    query.OrderByDescending(e => e.DocumentNumber) :
                    query.OrderBy(e => e.DocumentNumber),
                "birthday" => filter.SortDescending ? 
                    query.OrderByDescending(e => e.Birthday) :
                    query.OrderBy(e => e.Birthday),
                "costperhour" => filter.SortDescending ? 
                    query.OrderByDescending(e => e.CostPerHour) :
                    query.OrderBy(e => e.CostPerHour),
                "registerdate" => filter.SortDescending ? 
                    query.OrderByDescending(e => e.RegisterDate) :
                    query.OrderBy(e => e.RegisterDate),
                _ => filter.SortDescending ? 
                    query.OrderByDescending(e => e.RegisterDate) :
                    query.OrderBy(e => e.RegisterDate)
            };

            // Aplicar paginación
            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<EmployeeStatistics> GetEmployeeStatisticsAsync()
        {
            var totalEmployees = await _dbSet.CountAsync();
            var activeEmployees = await _dbSet.CountAsync(e => e.StateId == 1); // Ajustar según configuración
            var inactiveEmployees = totalEmployees - activeEmployees;
            var employeesWithFingerPrint = await _dbSet.CountAsync(e => e.FingerPrints != null && e.FingerPrints.Any());
            
            var averageCostPerHour = await _dbSet
                .Where(e => e.CostPerHour.HasValue)
                .AverageAsync(e => e.CostPerHour ?? 0);

            var averageAge = await _dbSet
                .Select(e => DateTime.Today.Year - e.Birthday.Year)
                .AverageAsync();

            return new EmployeeStatistics
            {
                TotalEmployees = totalEmployees,
                ActiveEmployees = activeEmployees,
                InactiveEmployees = inactiveEmployees,
                EmployeesWithFingerPrint = employeesWithFingerPrint,
                AverageCostPerHour = averageCostPerHour,
                AverageAge = (int)averageAge
            };
        }

        public async Task<IEnumerable<EmployeeInfo>> GetEmployeesWithoutAssistanceAsync(DateTime fromDate, DateTime toDate)
        {
            // Esta consulta requeriría un join con la tabla Assistance
            // Por ahora retornamos una lista vacía hasta implementar la lógica completa
            return await Task.FromResult(Enumerable.Empty<EmployeeInfo>());
        }

        public async Task<IEnumerable<EmployeeWorkSummary>> GetTopWorkingEmployeesAsync(DateTime fromDate, DateTime toDate, int top = 10)
        {
            // Esta consulta requeriría un join con las tablas Assistance y Projects
            // Por ahora retornamos una lista vacía hasta implementar la lógica completa
            return await Task.FromResult(Enumerable.Empty<EmployeeWorkSummary>());
        }
    }
}
