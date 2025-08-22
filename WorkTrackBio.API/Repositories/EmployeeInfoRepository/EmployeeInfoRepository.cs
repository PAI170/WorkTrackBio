using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data.Context;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.EmployeeInfoRepository
{
    public class EmployeeInfoRepository : IEmployeeInfoRepository
    {
        private readonly WorkTrackBioDbContext _context;

        public EmployeeInfoRepository(WorkTrackBioDbContext context)
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
                .FirstOrDefaultAsync(e => e.DocumentNumber.ToLower() == documentNumber.ToLower() && 
                                        e.DocumentTypeId == documentTypeId);
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber, int documentTypeId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return false;

            return await _context.EmployeeInfos
                .AnyAsync(e => e.DocumentNumber.ToLower() == documentNumber.ToLower() && 
                              e.DocumentTypeId == documentTypeId);
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
            var hasProjectsAssigns = await _context.ProjectsAssigns.AnyAsync(pa => pa.EmployeeId == employeeInfoId);
            var hasFingerPrint = await _context.FingerPrints.AnyAsync(fp => fp.EmployeeId == employeeInfoId);
            var hasAuditRegister = await _context.AuditRegisters.AnyAsync(ar => ar.AdminId == employeeInfoId);

            return hasAssistance || hasProjectsAssigns || hasFingerPrint || hasAuditRegister;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
