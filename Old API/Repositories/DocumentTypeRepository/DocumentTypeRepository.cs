using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.DocumentTypeRepository
{
    public class DocumentTypeRepository : IDocumentTypeRepository
    {
        private readonly AppDbContext _context;

        public DocumentTypeRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<DocumentType>> GetAllAsync()
        {
            return await _context.DocumentTypes.ToListAsync();
        }

        public async Task<DocumentType?> GetByIdAsync(int id)
        {
            return await _context.DocumentTypes.FindAsync(id);
        }

        public async Task<DocumentType?> GetByNameAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return null;

            return await _context.DocumentTypes
                .FirstOrDefaultAsync(dt => EF.Functions.Like(dt.DocumentName, documentName));
        }

        public async Task<bool> ExistsByNameAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return false;

            return await _context.DocumentTypes
                .AnyAsync(dt => EF.Functions.Like(dt.DocumentName, documentName));
        }

        public async Task<DocumentType> CreateAsync(DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException(nameof(documentType));

            _context.DocumentTypes.Add(documentType);
            await SaveChangesAsync();
            return documentType;
        }

        public async Task<DocumentType> UpdateAsync(DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException(nameof(documentType));

            _context.DocumentTypes.Update(documentType);
            await SaveChangesAsync();
            return documentType;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var documentType = await _context.DocumentTypes.FindAsync(id);
            if (documentType == null)
                return false;

            _context.DocumentTypes.Remove(documentType);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasDependenciesAsync(int documentTypeId)
        {
            var hasEmployeeInfo = await _context.EmployeeInfos.AnyAsync(ei => ei.DocumentTypeId == documentTypeId);
            return hasEmployeeInfo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
