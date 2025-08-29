using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectWarrantyRepository
{
    /// <summary>
    /// Implementación del repositorio para ProjectWarranty
    /// </summary>
    public class ProjectWarrantyRepository : IProjectWarrantyRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public ProjectWarrantyRepository(WorkTrackBioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ProjectWarranty>> GetAllAsync()
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .OrderByDescending(pw => pw.WarrantyDate)
                .ToListAsync();
        }

        public async Task<ProjectWarranty?> GetByIdAsync(int id)
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .FirstOrDefaultAsync(pw => pw.Id == id);
        }

        public async Task<IEnumerable<ProjectWarranty>> GetByProjectAsync(int projectId)
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .Where(pw => pw.IdProject == projectId)
                .OrderByDescending(pw => pw.WarrantyDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectWarranty>> GetByEmployeeAsync(int employeeId)
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .Where(pw => pw.MadeById == employeeId)
                .OrderByDescending(pw => pw.WarrantyDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectWarranty>> GetByStateAsync(int stateId)
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .Where(pw => pw.StateId == stateId)
                .OrderByDescending(pw => pw.WarrantyDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectWarranty>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .Where(pw => pw.WarrantyDate >= startDate && pw.WarrantyDate <= endDate)
                .OrderByDescending(pw => pw.WarrantyDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectWarranty>> GetByProjectAndStateAsync(int projectId, int stateId)
        {
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .Where(pw => pw.IdProject == projectId && pw.StateId == stateId)
                .OrderByDescending(pw => pw.WarrantyDate)
                .ToListAsync();
        }

        public async Task<ProjectWarranty> CreateAsync(ProjectWarranty projectWarranty)
        {
            if (projectWarranty == null)
                throw new ArgumentNullException(nameof(projectWarranty));

            _context.ProjectWarranties.Add(projectWarranty);
            await SaveChangesAsync();

            // Retornar el registro creado con las entidades relacionadas
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .FirstOrDefaultAsync(pw => pw.Id == projectWarranty.Id) ?? projectWarranty;
        }

        public async Task<ProjectWarranty> UpdateAsync(ProjectWarranty projectWarranty)
        {
            if (projectWarranty == null)
                throw new ArgumentNullException(nameof(projectWarranty));

            _context.ProjectWarranties.Update(projectWarranty);
            await SaveChangesAsync();

            // Retornar el registro actualizado con las entidades relacionadas
            return await _context.ProjectWarranties
                .Include(pw => pw.Project)
                .Include(pw => pw.MadeBy)
                .Include(pw => pw.State)
                .FirstOrDefaultAsync(pw => pw.Id == projectWarranty.Id) ?? projectWarranty;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var projectWarranty = await _context.ProjectWarranties.FindAsync(id);
            if (projectWarranty == null)
                return false;

            _context.ProjectWarranties.Remove(projectWarranty);
            await SaveChangesAsync();
            return true;
        }

        public Task<bool> HasDependenciesAsync(int projectWarrantyId)
        {
            // Por ahora no hay dependencias directas de ProjectWarranty
            // Se puede expandir en el futuro si se agregan más entidades relacionadas
            return Task.FromResult(false);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
