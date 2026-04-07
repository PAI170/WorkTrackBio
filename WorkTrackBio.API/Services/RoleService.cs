using Mapster;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Catalog;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RoleResponseDto>> GetAllAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    UserCount = r.AppUsers.Count(),
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDto> GetByIdAsync(int id)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .Include(r => r.AppUsers)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new NotFoundException("Rol no encontrado.");

            var dto = role.Adapt<RoleResponseDto>();
            dto.UserCount = role.AppUsers.Count;

            return dto;
        }

        public async Task<RoleResponseDto> CreateAsync(RoleCreateDto dto)
        {
            var nameExists = await _context.Roles
                .AnyAsync(r => EF.Functions.Collate(r.RoleName, "SQL_Latin1_General_CP1_CI_AI") == dto.RoleName);

            if (nameExists)
                throw new ConflictException("Ya existe un rol con ese nombre.");

            var role = dto.Adapt<Role>();

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var responseDto = role.Adapt<RoleResponseDto>();
            responseDto.UserCount = 0;

            return responseDto;
        }

        public async Task<RoleResponseDto> UpdateAsync(int id, RoleUpdateDto dto)
        {
            var role = await _context.Roles
                .Include(r => r.AppUsers)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new NotFoundException("Rol no encontrado.");

            var nameExists = await _context.Roles
                .AnyAsync(r => EF.Functions.Collate(r.RoleName, "SQL_Latin1_General_CP1_CI_AI") == dto.RoleName && r.Id != id);

            if (nameExists)
                throw new ConflictException("Ya existe un rol con ese nombre.");

            dto.Adapt(role);
            await _context.SaveChangesAsync();

            var responseDto = role.Adapt<RoleResponseDto>();
            responseDto.UserCount = role.AppUsers.Count;

            return responseDto;
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.AppUsers)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new NotFoundException("Rol no encontrado.");

            if (role.AppUsers.Any())
                throw new ConflictException(
                    "No se puede eliminar el rol porque tiene usuarios asignados.");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}