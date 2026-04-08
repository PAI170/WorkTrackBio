using Mapster;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.AppUser;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly AppDbContext _context;

        public AppUserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppUserListDto>> GetAllAsync()
        {
            return await _context.AppUsers
                .AsNoTracking()
                .Select(u => new AppUserListDto
                {
                    Id = u.Id,
                    FullName = u.EmployeeInfo.FirstName + " " + u.EmployeeInfo.LastName,
                    WorkEmail = u.WorkEmail,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<AppUserDetailDto> GetByIdAsync(int id)
        {
            var user = await _context.AppUsers
                .AsNoTracking()
                .Include(u => u.EmployeeInfo)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NotFoundException("Usuario no encontrado.");

            var dto = user.Adapt<AppUserDetailDto>();
            dto.FullName = $"{user.EmployeeInfo.FirstName} {user.EmployeeInfo.LastName}";
            dto.RoleName = user.Role.RoleName;

            return dto;
        }

        public async Task<AppUserDetailDto> CreateAsync(AppUserCreateDto dto)
        {
            var employee = await _context.EmployeeInfos
                .FirstOrDefaultAsync(e => e.Id == dto.EmployeeInfoId)
                ?? throw new NotFoundException("El empleado especificado no existe.");

            if (!employee.IsActive)
                throw new ConflictException("No se puede crear una cuenta para un empleado inactivo.");

            var alreadyHasUser = await _context.AppUsers
                .AnyAsync(u => u.EmployeeInfoId == dto.EmployeeInfoId);

            if (alreadyHasUser)
                throw new ConflictException("Este empleado ya tiene una cuenta de acceso.");

            if (!await _context.Roles.AnyAsync(r => r.Id == dto.RoleId))
                throw new NotFoundException("El rol especificado no existe.");

            var emailExists = await _context.AppUsers
                .AnyAsync(u => EF.Functions.Collate(u.WorkEmail, "SQL_Latin1_General_CP1_CI_AI") == dto.WorkEmail);

            if (emailExists)
                throw new ConflictException("Ya existe un usuario con ese correo de trabajo.");

            var (hash, salt) = PasswordHelper.HashPassword(dto.Password);

            var user = new AppUser
            {
                EmployeeInfoId = dto.EmployeeInfoId,
                RoleId = dto.RoleId,
                WorkEmail = dto.WorkEmail,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true
            };

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(user.Id);
        }

        public async Task<AppUserDetailDto> UpdateAsync(int id, AppUserUpdateDto dto)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NotFoundException("Usuario no encontrado.");

            if (!await _context.Roles.AnyAsync(r => r.Id == dto.RoleId))
                throw new NotFoundException("El rol especificado no existe.");

            var emailExists = await _context.AppUsers
                .AnyAsync(u => EF.Functions.Collate(u.WorkEmail, "SQL_Latin1_General_CP1_CI_AI") == dto.WorkEmail
                            && u.Id != id);

            if (emailExists)
                throw new ConflictException("Ya existe un usuario con ese correo de trabajo.");

            user.RoleId = dto.RoleId;
            user.WorkEmail = dto.WorkEmail;
            user.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(user.Id);
        }

        public async Task ResetPasswordAsync(int id, AppUserResetPasswordDto dto)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NotFoundException("Usuario no encontrado.");

            var (hash, salt) = PasswordHelper.HashPassword(dto.NewPassword);

            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await _context.SaveChangesAsync();
        }

        public async Task UnlockAsync(int id)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NotFoundException("Usuario no encontrado.");

            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new NotFoundException("Usuario no encontrado.");

            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}