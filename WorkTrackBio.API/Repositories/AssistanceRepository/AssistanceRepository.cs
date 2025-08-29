using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.AssistanceRepository
{
    /// <summary>
    /// Implementación del repositorio para Assistance
    /// </summary>
    public class AssistanceRepository : IAssistanceRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public AssistanceRepository(WorkTrackBioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Assistance>> GetAllAsync()
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<Assistance?> GetByIdAsync(int id)
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Assistance>> GetByEmployeeAsync(int employeeId)
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByProjectAsync(int projectId)
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.ProjectId == projectId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByDateAsync(DateOnly date)
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.CheckIn.Date == date.ToDateTime(TimeOnly.MinValue))
                .OrderBy(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.CheckIn >= startDateTime && a.CheckIn <= endDateTime)
                .OrderBy(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByEmployeeAndProjectAsync(int employeeId, int projectId)
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.EmployeeId == employeeId && a.ProjectId == projectId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.EmployeeId == employeeId && a.CheckIn >= startDateTime && a.CheckIn <= endDateTime)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByProjectAndDateRangeAsync(int projectId, DateOnly startDate, DateOnly endDate)
        {
            var startDateTime = startDate.ToDateTime(TimeOnly.MinValue);
            var endDateTime = endDate.ToDateTime(TimeOnly.MaxValue);

            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.ProjectId == projectId && a.CheckIn >= startDateTime && a.CheckIn <= endDateTime)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Assistance>> GetByProjectAndEmployeeAsync(int projectId, int employeeId)
        {
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .Where(a => a.ProjectId == projectId && a.EmployeeId == employeeId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> HasOpenCheckInAsync(int employeeId)
        {
            return await _context.Assistances
                .AnyAsync(a => a.EmployeeId == employeeId && a.CheckOut == null);
        }

        public async Task<Assistance> CreateAsync(Assistance assistance)
        {
            if (assistance == null)
                throw new ArgumentNullException(nameof(assistance));

            _context.Assistances.Add(assistance);
            await SaveChangesAsync();

            // Retornar el registro creado con las entidades relacionadas
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .FirstOrDefaultAsync(a => a.Id == assistance.Id) ?? assistance;
        }

        public async Task<Assistance> UpdateAsync(Assistance assistance)
        {
            if (assistance == null)
                throw new ArgumentNullException(nameof(assistance));

            // Establecer la fecha de modificación
            assistance.ModifiedDate = DateTime.UtcNow;

            _context.Assistances.Update(assistance);
            await SaveChangesAsync();

            // Retornar el registro actualizado con las entidades relacionadas
            return await _context.Assistances
                .Include(a => a.Employee)
                .Include(a => a.Project)
                .FirstOrDefaultAsync(a => a.Id == assistance.Id) ?? assistance;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assistance = await _context.Assistances.FindAsync(id);
            if (assistance == null)
                return false;

            _context.Assistances.Remove(assistance);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasDependenciesAsync(int assistanceId)
        {
            // Verificar si hay registros de auditoría que dependan de esta asistencia
            return await _context.AuditRegisters.AnyAsync(ar => ar.AssistanceId == assistanceId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
