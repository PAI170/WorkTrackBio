using Mapster;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Catalog;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;

        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentResponseDto>> GetAllAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .Select(d => new DepartmentResponseDto
                {
                    Id = d.Id,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    EmployeeCount = d.EmployeeInfos.Count(),
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<DepartmentResponseDto> GetByIdAsync(int id)
        {
            var department = await _context.Departments
                .AsNoTracking()
                .Include(d => d.EmployeeInfos)
                .FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new NotFoundException("Departamento no encontrado.");

            var dto = department.Adapt<DepartmentResponseDto>();
            dto.EmployeeCount = department.EmployeeInfos.Count;

            return dto;
        }

        public async Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto)
        {
            var nameExists = await _context.Departments
                .AnyAsync(d => EF.Functions.Collate(d.DepartmentName, "SQL_Latin1_General_CP1_CI_AI") == dto.DepartmentName);

            if (nameExists)
                throw new ConflictException("Ya existe un departamento con ese nombre.");

            var department = dto.Adapt<Department>();

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var responseDto = department.Adapt<DepartmentResponseDto>();
            responseDto.EmployeeCount = 0;

            return responseDto;
        }

        public async Task<DepartmentResponseDto> UpdateAsync(int id, DepartmentUpdateDto dto)
        {
            var department = await _context.Departments
                .Include(d => d.EmployeeInfos)
                .FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new NotFoundException("Departamento no encontrado.");

            var nameExists = await _context.Departments
                .AnyAsync(d => EF.Functions.Collate(d.DepartmentName, "SQL_Latin1_General_CP1_CI_AI") == dto.DepartmentName && d.Id != id);

            if (nameExists)
                throw new ConflictException("Ya existe un departamento con ese nombre.");

            dto.Adapt(department);
            await _context.SaveChangesAsync();

            var responseDto = department.Adapt<DepartmentResponseDto>();
            responseDto.EmployeeCount = department.EmployeeInfos.Count;

            return responseDto;
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments
                .Include(d => d.EmployeeInfos)
                .FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new NotFoundException("Departamento no encontrado.");

            if (department.EmployeeInfos.Any())
                throw new ConflictException(
                    "No se puede eliminar el departamento porque tiene empleados asignados.");

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
        }
    }
}