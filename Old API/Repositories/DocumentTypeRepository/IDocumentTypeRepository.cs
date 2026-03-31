using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.Repositories.DocumentTypeRepository
{
    public interface IDocumentTypeRepository
    {
        Task<IEnumerable<DocumentType>> GetAllAsync();
        Task<DocumentType?> GetByIdAsync(int id);
        Task<DocumentType?> GetByNameAsync(string documentName);
        Task<bool> ExistsByNameAsync(string documentName);
        Task<DocumentType> CreateAsync(DocumentType documentType);
        Task<DocumentType> UpdateAsync(DocumentType documentType);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasDependenciesAsync(int documentTypeId);
        Task<int> SaveChangesAsync();
    }
}
