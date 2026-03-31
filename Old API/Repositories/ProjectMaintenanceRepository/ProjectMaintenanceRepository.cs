using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectMaintenanceRepository
{
    public class ProjectMaintenanceRepository : IProjectMaintenanceRepository
    {
        private readonly AppDbContext _context;

        public ProjectMaintenanceRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ProjectMaintenance>> GetAllAsync()
        {
            return await _context.ProjectMaintenances
                .Include(pm => pm.Project)
                .Include(pm => pm.MadeBy)
                .Include(pm => pm.State)
                .OrderByDescending(pm => pm.MaintenanceDate)
                .ToListAsync();
        }

        public async Task<ProjectMaintenance?> GetByIdAsync(int id)
        {
            return await _context.ProjectMaintenances
                .Include(pm => pm.Project)
                .Include(pm => pm.MadeBy)
                .Include(pm => pm.State)
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task<IEnumerable<ProjectMaintenance>> GetByProjectAsync(int projectId)
        {
            return await _context.ProjectMaintenances
                .Include(pm => pm.Project)
                .Include(pm => pm.MadeBy)
                .Include(pm => pm.State)
                .Where(pm => pm.ProjectId == projectId)
                .OrderByDescending(pm => pm.MaintenanceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectMaintenance>> GetByEmployeeAsync(int employeeId)
        {
            return await _context.ProjectMaintenances
                .Include(pm => pm.Project)
                .Include(pm => pm.MadeBy)
                .Include(pm => pm.State)
                .Where(pm => pm.MadeById == employeeId)
                .OrderByDescending(pm => pm.MaintenanceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectMaintenance>> GetByStateAsync(int stateId)
        {
            return await _context.ProjectMaintenances
                .Include(pm => pm.Project)
                .Include(pm => pm.MadeBy)
                .Include(pm => pm.State)
                .Where(pm => pm.StateId == stateId)
                .OrderByDescending(pm => pm.MaintenanceDate)
                .ToListAsync();
        }

        public async Task<ProjectMaintenance> CreateAsync(ProjectMaintenance projectMaintenance)
        {
            if (projectMaintenance == null)
                throw new ArgumentNullException(nameof(projectMaintenance));

            _context.ProjectMaintenances.Add(projectMaintenance);
            await _context.SaveChangesAsync();
            return projectMaintenance;
        }

        public async Task<ProjectMaintenance?> UpdateAsync(ProjectMaintenance projectMaintenance)
        {
            if (projectMaintenance == null)
                throw new ArgumentNullException(nameof(projectMaintenance));

            var existingMaintenance = await _context.ProjectMaintenances.FindAsync(projectMaintenance.Id);
            if (existingMaintenance == null)
                return null;

            // Actualizar solo los campos que se proporcionan
            if (projectMaintenance.ProjectId > 0)
                existingMaintenance.ProjectId = projectMaintenance.ProjectId;
            
            if (!string.IsNullOrWhiteSpace(projectMaintenance.MaintenanceDescription))
                existingMaintenance.MaintenanceDescription = projectMaintenance.MaintenanceDescription;
            
            if (projectMaintenance.MadeById > 0)
                existingMaintenance.MadeById = projectMaintenance.MadeById;
            
            if (projectMaintenance.MaintenanceCost.HasValue)
                existingMaintenance.MaintenanceCost = projectMaintenance.MaintenanceCost;
            
            if (projectMaintenance.AdditionalInfo != null)
                existingMaintenance.AdditionalInfo = projectMaintenance.AdditionalInfo;
            
            if (projectMaintenance.StateId > 0)
                existingMaintenance.StateId = projectMaintenance.StateId;

            await _context.SaveChangesAsync();
            return existingMaintenance;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var maintenance = await _context.ProjectMaintenances.FindAsync(id);
            if (maintenance == null)
                return false;

            _context.ProjectMaintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.ProjectMaintenances.AnyAsync(pm => pm.Id == id);
        }

        public Task<bool> HasDependenciesAsync(int id)
        {

            
            return Task.FromResult(false);
        }
    }
}
