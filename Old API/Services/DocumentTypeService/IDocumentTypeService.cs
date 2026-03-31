using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.DocumentType;

namespace WorkTrackBio.API.Services.DocumentTypeService
{
    public interface IDocumentTypeService
    {
        Task<ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>> GetAllDocumentTypesAsync();
        Task<ApiResponse<DocumentTypeDataTransferObject>> GetDocumentTypeByIdAsync(int id);
        Task<ApiResponse<DocumentTypeDataTransferObject>> GetDocumentTypeByNameAsync(string documentName);
        Task<ApiResponse<bool>> DocumentTypeExistsAsync(int id);
        Task<ApiResponse<bool>> DocumentTypeNameExistsAsync(string documentName);
        Task<ApiResponse<DocumentTypeDataTransferObject>> CreateDocumentTypeAsync(CreateDocumentTypeDataTransferObject createDto);
        Task<ApiResponse<DocumentTypeDataTransferObject>> UpdateDocumentTypeAsync(UpdateDocumentTypeDataTransferObject updateDto);
        Task<ApiResponse<bool>> DeleteDocumentTypeAsync(int id);
    }
}