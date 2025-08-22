using AutoMapper;
using WorkTrackBio.API.DataTransferObjects.DocumentType;
using WorkTrackBio.API.Repositories.DocumentTypeRepository;

namespace WorkTrackBio.API.Services.DocumentTypeService
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IMapper _mapper;

        public DocumentTypeService(IDocumentTypeRepository documentTypeRepository, IMapper mapper)
        {
            _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<DocumentTypeDataTransferObject>> GetAllDocumentTypesAsync()
        {
            var documentTypes = await _documentTypeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DocumentTypeDataTransferObject>>(documentTypes);
        }

        public async Task<DocumentTypeDataTransferObject?> GetDocumentTypeByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            return _mapper.Map<DocumentTypeDataTransferObject>(documentType);
        }

        public async Task<DocumentTypeDataTransferObject?> GetDocumentTypeByNameAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                throw new ArgumentException("El nombre del tipo de documento no puede estar vacío", nameof(documentName));

            var documentType = await _documentTypeRepository.GetByNameAsync(documentName);
            return _mapper.Map<DocumentTypeDataTransferObject>(documentType);
        }

        public async Task<bool> DocumentTypeExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            return documentType != null;
        }

        public async Task<bool> DocumentTypeNameExistsAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return false;

            return await _documentTypeRepository.ExistsByNameAsync(documentName);
        }

        public async Task<DocumentTypeDataTransferObject> CreateDocumentTypeAsync(CreateDocumentTypeDataTransferObject createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            var existingDocumentTypes = await _documentTypeRepository.GetAllAsync();
            if (existingDocumentTypes.Any(dt => string.Equals(dt.DocumentName, createDto.DocumentName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ya existe un tipo de documento con el nombre '{createDto.DocumentName}' (ignorando mayúsculas/minúsculas)");

            var documentType = _mapper.Map<WorkTrackBio.API.Data.Models.DocumentType>(createDto);
            var createdDocumentType = await _documentTypeRepository.CreateAsync(documentType);

            return _mapper.Map<DocumentTypeDataTransferObject>(createdDocumentType);
        }

        public async Task<DocumentTypeDataTransferObject?> UpdateDocumentTypeAsync(UpdateDocumentTypeDataTransferObject updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(updateDto.Id));

            var existingDocumentType = await _documentTypeRepository.GetByIdAsync(updateDto.Id);
            if (existingDocumentType == null)
                return null;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(updateDto.DocumentName))
            {
                var allDocumentTypes = await _documentTypeRepository.GetAllAsync();
                if (allDocumentTypes.Any(dt => string.Equals(dt.DocumentName, updateDto.DocumentName, StringComparison.OrdinalIgnoreCase) &&
                    dt.Id != updateDto.Id))
                    throw new InvalidOperationException($"Ya existe un tipo de documento con el nombre '{updateDto.DocumentName}' (ignorando mayúsculas/minúsculas)");

                if (!string.Equals(existingDocumentType.DocumentName, updateDto.DocumentName, StringComparison.OrdinalIgnoreCase))
                {
                    existingDocumentType.DocumentName = updateDto.DocumentName;
                    hasChanges = true;
                }
            }

            if (updateDto.Description != existingDocumentType.Description)
            {
                existingDocumentType.Description = updateDto.Description;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return _mapper.Map<DocumentTypeDataTransferObject>(existingDocumentType);
            }

            var updatedDocumentType = await _documentTypeRepository.UpdateAsync(existingDocumentType);
            return _mapper.Map<DocumentTypeDataTransferObject>(updatedDocumentType);
        }

        public async Task<bool> DeleteDocumentTypeAsync(int id)
        {
            if (id <= 0)
                return false;

            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            if (documentType == null)
                return false;

            var isDocumentTypeInUse = await IsDocumentTypeInUseAsync(id);
            if (isDocumentTypeInUse)
                throw new InvalidOperationException($"No se puede eliminar el tipo de documento '{documentType.DocumentName}' porque está siendo usado por empleados del sistema");

            return await _documentTypeRepository.DeleteAsync(id);
        }

        private async Task<bool> IsDocumentTypeInUseAsync(int documentTypeId)
        {
            var hasDependencies = await _documentTypeRepository.HasDependenciesAsync(documentTypeId);
            return hasDependencies;
        }
    }
}
