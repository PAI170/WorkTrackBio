using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.InternUserRepository
{
    public class InternUserRepository : IInternUserRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public InternUserRepository(WorkTrackBioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<InternUser>> GetAllAsync()
        {
            return await _context.InternUsers
                .Include(u => u.Role)
                .Include(u => u.State)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<InternUser?> GetByIdAsync(int id)
        {
            return await _context.InternUsers
                .Include(u => u.Role)
                .Include(u => u.State)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<InternUser?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _context.InternUsers
                .Include(u => u.Role)
                .Include(u => u.State)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<InternUser>> GetByRoleAsync(int roleId)
        {
            return await _context.InternUsers
                .Include(u => u.Role)
                .Include(u => u.State)
                .Where(u => u.RolId == roleId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<InternUser>> GetByStateAsync(int stateId)
        {
            return await _context.InternUsers
                .Include(u => u.Role)
                .Include(u => u.State)
                .Where(u => u.StateId == stateId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _context.InternUsers
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.InternUsers
                .AnyAsync(u => u.Id == id);
        }

        public async Task<InternUser> CreateAsync(InternUser internUser)
        {
            if (internUser == null)
                throw new ArgumentNullException(nameof(internUser));

            // Generar hash y salt de contraseña (placeholder por ahora)
            internUser.PasswordHash = "placeholder_hash";
            internUser.PasswordSalt = "placeholder_salt";
            
            // Establecer fecha de creación
            internUser.CreationDate = DateTime.UtcNow;

            _context.InternUsers.Add(internUser);
            await _context.SaveChangesAsync();

            // Recargar con las relaciones
            return await GetByIdAsync(internUser.Id) ?? internUser;
        }

        public async Task<InternUser> UpdateAsync(InternUser internUser)
        {
            if (internUser == null)
                throw new ArgumentNullException(nameof(internUser));

            var existingUser = await _context.InternUsers.FindAsync(internUser.Id);
            if (existingUser == null)
                throw new InvalidOperationException($"No se encontró un usuario interno con ID {internUser.Id}");

            // Actualizar solo los campos que han cambiado
            if (!string.IsNullOrWhiteSpace(internUser.Email) && existingUser.Email != internUser.Email)
                existingUser.Email = internUser.Email;

            if (!string.IsNullOrWhiteSpace(internUser.FirstName) && existingUser.FirstName != internUser.FirstName)
                existingUser.FirstName = internUser.FirstName;

            if (!string.IsNullOrWhiteSpace(internUser.LastName) && existingUser.LastName != internUser.LastName)
                existingUser.LastName = internUser.LastName;

            if (internUser.RolId > 0 && existingUser.RolId != internUser.RolId)
                existingUser.RolId = internUser.RolId;

            if (internUser.StateId > 0 && existingUser.StateId != internUser.StateId)
                existingUser.StateId = internUser.StateId;

            _context.InternUsers.Update(existingUser);
            await _context.SaveChangesAsync();

            // Recargar con las relaciones
            return await GetByIdAsync(existingUser.Id) ?? existingUser;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var internUser = await _context.InternUsers.FindAsync(id);
            if (internUser == null)
                return false;

            _context.InternUsers.Remove(internUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasDependenciesAsync(int internUserId)
        {
            // Verificar si el usuario tiene sesiones activas
            var hasSessions = await _context.Sessions
                .AnyAsync(s => s.UserId == internUserId);

            // Aquí puedes agregar más verificaciones de dependencias si es necesario
            // Por ejemplo, si el usuario está asignado a proyectos, etc.

            return hasSessions;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
