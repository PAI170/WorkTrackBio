using AutoMapper;
using WorkTrackBio.API.Common;
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

        public async Task<ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>> GetAllDocumentTypesAsync()
        {
            var documentTypes = await _documentTypeRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<DocumentTypeDataTransferObject>>(documentTypes);
            return ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>.SuccessResponse(result, "Tipos de documento obtenidos exitosamente");
        }

        public async Task<ApiResponse<DocumentTypeDataTransferObject>> GetDocumentTypeByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var documentType = await _documentTypeRepository.GetByIdAsync(id);

            if (documentType == null)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse($"No se encontró un tipo de documento con ID {id}", 404);

            var result = _mapper.Map<DocumentTypeDataTransferObject>(documentType);
            return ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(result, "Tipo de documento obtenido exitosamente");
        }

        public async Task<ApiResponse<DocumentTypeDataTransferObject>> GetDocumentTypeByNameAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("El nombre del tipo de documento no puede estar vacío", 400);

            var documentType = await _documentTypeRepository.GetByNameAsync(documentName);

            if (documentType == null)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse($"No se encontró un tipo de documento con el nombre '{documentName}'", 404);

            var result = _mapper.Map<DocumentTypeDataTransferObject>(documentType);
            return ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(result, "Tipo de documento obtenido exitosamente");
        }

        public async Task<ApiResponse<bool>> DocumentTypeExistsAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            var exists = documentType != null;
            return ApiResponse<bool>.SuccessResponse(exists, exists ? "El tipo de documento existe" : "El tipo de documento no existe");
        }

        public async Task<ApiResponse<bool>> DocumentTypeNameExistsAsync(string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
                return ApiResponse<bool>.ErrorResponse("El nombre no puede estar vacío", 400);

            var exists = await _documentTypeRepository.ExistsByNameAsync(documentName);
            return ApiResponse<bool>.SuccessResponse(exists, exists ? $"Ya existe un tipo de documento con el nombre '{documentName}'" : $"No existe un tipo de documento con el nombre '{documentName}'");
        }

        public async Task<ApiResponse<DocumentTypeDataTransferObject>> CreateDocumentTypeAsync(CreateDocumentTypeDataTransferObject createDto)
        {
            if (createDto == null)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("Los datos del tipo de documento no pueden estar vacíos", 400);
                
            if (await _documentTypeRepository.ExistsByNameAsync(createDto.DocumentName))
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse($"Ya existe un tipo de documento con el nombre '{createDto.DocumentName}'", 409);

            var documentType = _mapper.Map<WorkTrackBio.API.Data.Models.DocumentType>(createDto);
            var createdDocumentType = await _documentTypeRepository.CreateAsync(documentType);
            var result = _mapper.Map<DocumentTypeDataTransferObject>(createdDocumentType);

            return ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(result, "Tipo de documento creado exitosamente", 201);
        }

        public async Task<ApiResponse<DocumentTypeDataTransferObject>> UpdateDocumentTypeAsync(UpdateDocumentTypeDataTransferObject updateDto)
        {
            if (updateDto == null)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("Los datos del tipo de documento no pueden estar vacíos", 400);

            if (updateDto.Id <= 0)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var existingDocumentType = await _documentTypeRepository.GetByIdAsync(updateDto.Id);
            if (existingDocumentType == null)
                return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse($"No se encontró un tipo de documento con ID {updateDto.Id}", 404);

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(updateDto.DocumentName))
            {
                if (await _documentTypeRepository.ExistsByNameAsync(updateDto.DocumentName) && existingDocumentType.DocumentName.ToLower() != updateDto.DocumentName.ToLower())
                    return ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse($"Ya existe un tipo de documento con el nombre '{updateDto.DocumentName}'", 409);

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
                var unchanged = _mapper.Map<DocumentTypeDataTransferObject>(existingDocumentType);
                return ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(unchanged, "No se detectaron cambios");
            }

            var updatedDocumentType = await _documentTypeRepository.UpdateAsync(existingDocumentType);
            var result = _mapper.Map<DocumentTypeDataTransferObject>(updatedDocumentType);
            return ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(result, "Tipo de documento actualizado exitosamente");
        }

        public async Task<ApiResponse<bool>> DeleteDocumentTypeAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var documentType = await _documentTypeRepository.GetByIdAsync(id);
            if (documentType == null)
                return ApiResponse<bool>.ErrorResponse($"No se encontró un tipo de documento con ID {id}", 404);

            var isInUse = await _documentTypeRepository.HasDependenciesAsync(id);
            if (isInUse)
                return ApiResponse<bool>.ErrorResponse($"No se puede eliminar el tipo de documento '{documentType.DocumentName}' porque está siendo usado por empleados", 409);

            await _documentTypeRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Tipo de documento eliminado exitosamente");
        }
    }
}