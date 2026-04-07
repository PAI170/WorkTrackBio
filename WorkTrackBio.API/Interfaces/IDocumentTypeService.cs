using WorkTrackBio.API.DataTransferObjects.Catalog;

namespace WorkTrackBio.API.Interfaces
{
    public interface IDocumentTypeService
    {
        Task<IEnumerable<DocumentTypeResponseDto>> GetAllAsync();
        Task<DocumentTypeResponseDto> GetByIdAsync(int id);
        Task<DocumentTypeResponseDto> CreateAsync(DocumentTypeCreateDto dto);
        Task<DocumentTypeResponseDto> UpdateAsync(int id, DocumentTypeUpdateDto dto);
        Task DeleteAsync(int id);
    }
}