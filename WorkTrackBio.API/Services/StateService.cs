using Mapster;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Catalog;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class StateService : IStateService
    {
        private readonly AppDbContext _context;

        public StateService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StateResponseDto>> GetAllAsync()
        {
            var states = await _context.States
                .AsNoTracking()
                .ToListAsync();

            return states.Adapt<IEnumerable<StateResponseDto>>();
        }

        public async Task<StateResponseDto> GetByIdAsync(int id)
        {
            var state = await _context.States
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new NotFoundException("Estado no encontrado.");

            return state.Adapt<StateResponseDto>();
        }

        public async Task<StateResponseDto> CreateAsync(StateCreateDto dto)
        {
            var exists = await _context.States
                .AnyAsync(s => s.StateName == dto.StateName
                           && s.StateType == dto.StateType);

            if (exists)
                throw new ConflictException(
                    $"Ya existe un estado '{dto.StateName}' de tipo {dto.StateType}.");

            var state = dto.Adapt<State>();
            _context.States.Add(state);
            await _context.SaveChangesAsync();

            return state.Adapt<StateResponseDto>();
        }

        public async Task<StateResponseDto> UpdateAsync(int id, StateUpdateDto dto)
        {
            var state = await _context.States
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new NotFoundException("Estado no encontrado.");

            var exists = await _context.States
                .AnyAsync(s => s.StateName == dto.StateName
                           && s.StateType == dto.StateType
                           && s.Id != id);

            if (exists)
                throw new ConflictException(
                    $"Ya existe un estado '{dto.StateName}' de tipo {dto.StateType}.");

            dto.Adapt(state);
            await _context.SaveChangesAsync();

            return state.Adapt<StateResponseDto>();
        }
        public async Task DeleteAsync(int id)
        {
            var state = await _context.States
                .Include(s => s.Projects)
                .Include(s => s.EmployeeInfos)
                .Include(s => s.ProjectMaintenances)
                .Include(s => s.ProjectWarranties)
                .Include(s => s.TravelExpenses)
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new NotFoundException("Estado no encontrado.");

            var relationsInUse = new List<string>();

            if (state.Projects.Any()) relationsInUse.Add("Proyectos");
            if (state.EmployeeInfos.Any()) relationsInUse.Add("Empleados");
            if (state.ProjectMaintenances.Any()) relationsInUse.Add("Mantenimientos");
            if (state.ProjectWarranties.Any()) relationsInUse.Add("Garantías");
            if (state.TravelExpenses.Any()) relationsInUse.Add("Gastos de viaje");

            if (relationsInUse.Any())
                throw new ConflictException(
                    $"No se puede eliminar el estado porque está en uso: " +
                    string.Join(", ", relationsInUse) + "."
                );

            _context.States.Remove(state);
            await _context.SaveChangesAsync();
        }
    }
}