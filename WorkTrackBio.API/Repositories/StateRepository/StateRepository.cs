using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.StateRepository
{

    //Repository, how to do the requests to the DB
    public class StateRepository : IStateRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public StateRepository(WorkTrackBioDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<State>> GetAllAsync()
        {
            return await _context.States
                .AsNoTracking()
                .OrderBy(s => s.StateName)
                .ToListAsync();
        }

        public async Task<State?> GetByIdAsync(int id)
        {
            return await _context.States
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<State>> GetByTypeAsync(string stateType)
        {
            return await _context.States
                .AsNoTracking()
                .Where(s => s.StateType == stateType)
                .OrderBy(s => s.StateName)
                .ToListAsync();
        }

        public async Task<bool> ExistsByNameAsync(string stateName)
        {
            return await _context.States
                .AsNoTracking()
                .AnyAsync(s => s.StateName == stateName);
        }

        public async Task<State> CreateAsync(State state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            _context.States.Add(state);
            await SaveChangesAsync();

            return state;
        }

        public async Task<State> UpdateAsync(State state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            var existingState = await _context.States.FindAsync(state.Id);
            if (existingState == null)
                throw new InvalidOperationException($"No se encontró el estado con ID {state.Id}");

            _context.Entry(existingState).CurrentValues.SetValues(state);
            await SaveChangesAsync();

            return existingState;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var state = await _context.States.FindAsync(id);
            if (state == null)
                return false;

            _context.States.Remove(state);
            await SaveChangesAsync();

            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
