using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.ProjectRepository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public ProjectRepository(WorkTrackBioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects
                .Include(p => p.State)
                .ToListAsync();
        }

        public async Task<Project?> GetByIdAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.State)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project?> GetByNameAsync(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return null;

            return await _context.Projects
                .Include(p => p.State)
                .FirstOrDefaultAsync(p => p.ProjectName.ToLower() == projectName.ToLower());
        }

        public async Task<bool> ExistsByNameAsync(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return false;

            return await _context.Projects
                .AnyAsync(p => p.ProjectName.ToLower() == projectName.ToLower());
        }

        public async Task<IEnumerable<Project>> GetByStateAsync(int stateId)
        {
            return await _context.Projects
                .Include(p => p.State)
                .Where(p => p.StateId == stateId)
                .ToListAsync();
        }

        public async Task<Project> CreateAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            _context.Projects.Add(project);
            await SaveChangesAsync();
            return project;
        }

        public async Task<Project> UpdateAsync(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            // El proyecto que viene del Service ya tiene los cambios aplicados
            // Solo necesitamos marcarlo como modificado y guardar
            _context.Projects.Update(project);
            await SaveChangesAsync();
            
            // Retornar el proyecto con la navegación State incluida
            return await _context.Projects
                .Include(p => p.State)
                .FirstOrDefaultAsync(p => p.Id == project.Id) ?? project;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            _context.Projects.Remove(project);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasDependenciesAsync(int projectId)
        {
            var hasAssistance = await _context.Assistances.AnyAsync(a => a.ProjectId == projectId);
            var hasDevices = await _context.Devices.AnyAsync(d => d.ProjectId == projectId);
            var hasProjectsAssigns = await _context.ProjectsAssigns.AnyAsync(pa => pa.ProjectId == projectId);
            var hasProjectMaintenance = await _context.ProjectMaintenances.AnyAsync(pm => pm.IdProject == projectId);
            var hasProjectWarranty = await _context.ProjectWarranties.AnyAsync(pw => pw.IdProject == projectId);

            return hasAssistance || hasDevices || hasProjectsAssigns || hasProjectMaintenance || hasProjectWarranty;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
