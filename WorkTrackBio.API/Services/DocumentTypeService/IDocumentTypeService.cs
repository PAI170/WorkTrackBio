using WorkTrackBio.API.DataTransferObjects.DocumentType;

namespace WorkTrackBio.API.Services.DocumentTypeService
{
    public interface IDocumentTypeService
    {
        Task<IEnumerable<DocumentTypeDataTransferObject>> GetAllDocumentTypesAsync();
        Task<DocumentTypeDataTransferObject?> GetDocumentTypeByIdAsync(int id);
        Task<DocumentTypeDataTransferObject?> GetDocumentTypeByNameAsync(string documentName);
        Task<bool> DocumentTypeExistsAsync(int id);
        Task<bool> DocumentTypeNameExistsAsync(string documentName);
        Task<DocumentTypeDataTransferObject> CreateDocumentTypeAsync(CreateDocumentTypeDataTransferObject createDto);
        Task<DocumentTypeDataTransferObject?> UpdateDocumentTypeAsync(UpdateDocumentTypeDataTransferObject updateDto);
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}
