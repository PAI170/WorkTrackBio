using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.RoleRepository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public RoleRepository(WorkTrackBioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            return await _context.Roles
                .FirstOrDefaultAsync(r => EF.Functions.Like(r.RoleName, roleName));
        }

        public async Task<bool> ExistsByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            return await _context.Roles
                .AnyAsync(r => EF.Functions.Like(r.RoleName, roleName));
        }

        public async Task<Role> CreateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            _context.Roles.Add(role);
            await SaveChangesAsync();
            return role;
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var existingRole = await _context.Roles.FindAsync(role.Id);
            if (existingRole == null)
                throw new InvalidOperationException($"No se encontró el rol con ID {role.Id}");

            bool hasChanges = false;

            if (existingRole.RoleName != role.RoleName)
            {
                existingRole.RoleName = role.RoleName;
                hasChanges = true;
            }

            if (existingRole.Description != role.Description)
            {
                existingRole.Description = role.Description;
                hasChanges = true;
            }

            if (hasChanges)
            {
                await SaveChangesAsync();
            }

            return existingRole;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return false;

            _context.Roles.Remove(role);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasDependenciesAsync(int roleId)
        {
            var hasInternUsers = await _context.InternUsers.AnyAsync(u => u.RolId == roleId);
            return hasInternUsers;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
