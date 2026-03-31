using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.EmployeeInfoRepository
{
    public class EmployeeInfoRepository : IEmployeeInfoRepository
    {
        private readonly AppDbContext _context;

        public EmployeeInfoRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<EmployeeInfo>> GetAllAsync()
        {
            return await _context.EmployeeInfos
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .ToListAsync();
        }

        public async Task<EmployeeInfo?> GetByIdAsync(int id)
        {
            return await _context.EmployeeInfos
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EmployeeInfo?> GetByDocumentNumberAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return null;

            return await _context.EmployeeInfos
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .FirstOrDefaultAsync(e => EF.Functions.Like(e.DocumentNumber, documentNumber)
                                     && e.DocumentTypeId == documentTypeId);
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            return await _context.EmployeeInfos
                .AnyAsync(e => EF.Functions.Like(e.DocumentNumber, documentNumber)
                          && e.DocumentTypeId == documentTypeId);
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByStateAsync(int stateId)
        {
            return await _context.EmployeeInfos
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .Where(e => e.StateId == stateId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeInfo>> GetByDocumentTypeAsync(int documentTypeId)
        {
            return await _context.EmployeeInfos
                .Include(e => e.DocumentType)
                .Include(e => e.State)
                .Where(e => e.DocumentTypeId == documentTypeId)
                .ToListAsync();
        }

        public async Task<EmployeeInfo> CreateAsync(EmployeeInfo employeeInfo)
        {
            if (employeeInfo == null)
                throw new ArgumentNullException(nameof(employeeInfo));

            _context.EmployeeInfos.Add(employeeInfo);
            await SaveChangesAsync();
            return employeeInfo;
        }

        public async Task<EmployeeInfo> UpdateAsync(EmployeeInfo employeeInfo)
        {
            if (employeeInfo == null)
                throw new ArgumentNullException(nameof(employeeInfo));

            _context.EmployeeInfos.Update(employeeInfo);
            await SaveChangesAsync();
            return employeeInfo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employeeInfo = await _context.EmployeeInfos.FindAsync(id);
            if (employeeInfo == null)
                return false;

            _context.EmployeeInfos.Remove(employeeInfo);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasDependenciesAsync(int employeeInfoId)
        {
            var hasAssistance = await _context.Assistances.AnyAsync(a => a.EmployeeId == employeeInfoId);


            return hasAssistance;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
