using Mapster;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Employee;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeListDto>> GetAllAsync()
        {
            return await _context.EmployeeInfos
                .AsNoTracking()
                .Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    FullName = e.FirstName + " " + e.LastName,
                    Position = e.Position,
                    DepartmentName = e.Department.DepartmentName,
                    StateName = e.State.StateName,
                    IsActive = e.IsActive,
                    PhotoUrl = e.PhotoUrl
                })
                .ToListAsync();
        }

        public async Task<EmployeeDetailDto> GetByIdAsync(int id)
        {
            var employee = await _context.EmployeeInfos
                .AsNoTracking()
                .Include(e => e.State)
                .Include(e => e.Department)
                .Include(e => e.DocumentType)
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NotFoundException("Empleado no encontrado.");

            var dto = employee.Adapt<EmployeeDetailDto>();

            dto.StateName = employee.State.StateName;
            dto.DepartmentName = employee.Department.DepartmentName;
            dto.DocumentTypeName = employee.DocumentType.DocumentName;

            return dto;
        }

        public async Task<EmployeeDetailDto> CreateAsync(EmployeeCreateDto dto)
        {
            await ValidateForeignKeysAsync(dto.StateId, dto.DepartmentId, dto.DocumentTypeId);
            await ValidateUniqueFieldsAsync(dto.DocumentNumber, dto.PersonalEmail);

            var employee = dto.Adapt<EmployeeInfo>();
            employee.IsActive = true;

            _context.EmployeeInfos.Add(employee);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(employee.Id);
        }

        public async Task<EmployeeDetailDto> UpdateAsync(int id, EmployeeUpdateDto dto)
        {
            var employee = await _context.EmployeeInfos
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NotFoundException("Empleado no encontrado.");

            await ValidateForeignKeysAsync(dto.StateId, dto.DepartmentId, dto.DocumentTypeId);
            await ValidateUniqueFieldsAsync(dto.DocumentNumber, dto.PersonalEmail, id);

            dto.Adapt(employee);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(employee.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.EmployeeInfos
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NotFoundException("Empleado no encontrado.");

            _context.EmployeeInfos.Remove(employee);
            await _context.SaveChangesAsync();
        }

        private async Task ValidateForeignKeysAsync(int stateId, int departmentId, int documentTypeId)
        {
            if (!await _context.States.AnyAsync(s => s.Id == stateId))
                throw new NotFoundException("El estado especificado no existe.");

            if (!await _context.Departments.AnyAsync(d => d.Id == departmentId))
                throw new NotFoundException("El departamento especificado no existe.");

            if (!await _context.DocumentTypes.AnyAsync(d => d.Id == documentTypeId))
                throw new NotFoundException("El tipo de documento especificado no existe.");
        }

        private async Task ValidateUniqueFieldsAsync(string documentNumber, string personalEmail, int? excludeId = null)
        {
            var docExists = await _context.EmployeeInfos
                .AnyAsync(e => EF.Functions.Collate(e.DocumentNumber, "SQL_Latin1_General_CP1_CI_AI") == documentNumber
                            && (excludeId == null || e.Id != excludeId));

            if (docExists)
                throw new ConflictException("Ya existe un empleado con ese número de documento.");

            var emailExists = await _context.EmployeeInfos
                .AnyAsync(e => EF.Functions.Collate(e.PersonalEmail, "SQL_Latin1_General_CP1_CI_AI") == personalEmail
                            && (excludeId == null || e.Id != excludeId));

            if (emailExists)
                throw new ConflictException("Ya existe un empleado con ese correo electrónico.");
        }
    }
}