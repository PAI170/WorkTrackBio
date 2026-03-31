using FluentValidation.Results;
using WorkTrackBio.API.DataTransferObjects.DocumentType;

namespace WorkTrackBio.API.Validators.DocumentTypeValidator
{
    public interface IDocumentTypeValidator
    {
        Task<ValidationResult> ValidateCreateAsync(CreateDocumentTypeDataTransferObject createDto);
        Task<ValidationResult> ValidateUpdateAsync(UpdateDocumentTypeDataTransferObject updateDto);
        ValidationResult ValidateId(int id);
        ValidationResult ValidateDocumentName(string documentName);
        Task<ValidationResult> ValidateDocumentTypeExistsAsync(int id);
        Task<ValidationResult> ValidateDocumentTypeNameUniqueAsync(string documentName, int? excludeId = null);
    }
}
