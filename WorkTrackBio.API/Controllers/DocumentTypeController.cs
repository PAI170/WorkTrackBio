using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.DocumentType;
using WorkTrackBio.API.Services.DocumentTypeService;
using WorkTrackBio.API.Validators.DocumentTypeValidator;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IDocumentTypeValidator _documentTypeValidator;

        public DocumentTypeController(IDocumentTypeService documentTypeService, IDocumentTypeValidator documentTypeValidator)
        {
            _documentTypeService = documentTypeService ?? throw new ArgumentNullException(nameof(documentTypeService));
            _documentTypeValidator = documentTypeValidator ?? throw new ArgumentNullException(nameof(documentTypeValidator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>>> GetAllDocumentTypes()
        {
            try
            {
                var documentTypes = await _documentTypeService.GetAllDocumentTypesAsync();
                var response = ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>.SuccessResponse(
                    documentTypes, 
                    "Tipos de documento obtenidos exitosamente", 
                    StatusCodes.Status200OK);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> GetDocumentTypeById(int id)
        {
            try
            {
                var idValidation = _documentTypeValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var documentType = await _documentTypeService.GetDocumentTypeByIdAsync(id);
                
                if (documentType == null)
                {
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        $"No se encontró un tipo de documento con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(
                    documentType, 
                    "Tipo de documento obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("name/{documentName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> GetDocumentTypeByName(string documentName)
        {
            try
            {
                var nameValidation = _documentTypeValidator.ValidateDocumentName(documentName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        "Nombre de tipo de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var documentType = await _documentTypeService.GetDocumentTypeByNameAsync(documentName);
                
                if (documentType == null)
                {
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        $"No se encontró un tipo de documento con el nombre '{documentName}'", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(
                    documentType, 
                    "Tipo de documento obtenido exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (ArgumentException ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status400BadRequest);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DocumentTypeExists(int id)
        {
            try
            {
                var idValidation = _documentTypeValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _documentTypeService.DocumentTypeExistsAsync(id);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? "El tipo de documento existe" : "El tipo de documento no existe", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("name/{documentName}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<bool>>> DocumentTypeNameExists(string documentName)
        {
            try
            {
                var nameValidation = _documentTypeValidator.ValidateDocumentName(documentName);
                if (!nameValidation.IsValid)
                {
                    var errors = nameValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<bool>.ErrorResponse(
                        "Nombre de tipo de documento inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var exists = await _documentTypeService.DocumentTypeNameExistsAsync(documentName);
                var successResponse = ApiResponse<bool>.SuccessResponse(
                    exists, 
                    exists ? $"Ya existe un tipo de documento con el nombre '{documentName}'" : $"No existe un tipo de documento con el nombre '{documentName}'", 
                    StatusCodes.Status200OK);
                
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> CreateDocumentType(CreateDocumentTypeDataTransferObject createDto)
        {
            try
            {
                var validation = await _documentTypeValidator.ValidateCreateAsync(createDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        "Datos de tipo de documento inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var createdDocumentType = await _documentTypeService.CreateDocumentTypeAsync(createDto);
                var successResponse = ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(
                    createdDocumentType, 
                    "Tipo de documento creado exitosamente", 
                    StatusCodes.Status201Created);
                
                return CreatedAtAction(nameof(GetDocumentTypeById), new { id = createdDocumentType.Id }, successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> UpdateDocumentType(UpdateDocumentTypeDataTransferObject updateDto)
        {
            try
            {
                var validation = await _documentTypeValidator.ValidateUpdateAsync(updateDto);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        "Datos de tipo de documento inválidos", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var updatedDocumentType = await _documentTypeService.UpdateDocumentTypeAsync(updateDto);
                
                if (updatedDocumentType == null)
                {
                    var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                        $"No se encontró un tipo de documento con ID {updateDto.Id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<DocumentTypeDataTransferObject>.SuccessResponse(
                    updatedDocumentType, 
                    "Tipo de documento actualizado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteDocumentType(int id)
        {
            try
            {
                var idValidation = _documentTypeValidator.ValidateId(id);
                if (!idValidation.IsValid)
                {
                    var errors = idValidation.Errors.Select(e => e.ErrorMessage).ToList();
                    var response = ApiResponse<string>.ErrorResponse(
                        "ID inválido", 
                        StatusCodes.Status400BadRequest, 
                        errors);
                    return BadRequest(response);
                }

                var deleted = await _documentTypeService.DeleteDocumentTypeAsync(id);
                
                if (!deleted)
                {
                    var response = ApiResponse<string>.ErrorResponse(
                        $"No se encontró un tipo de documento con ID {id}", 
                        StatusCodes.Status404NotFound);
                    return NotFound(response);
                }

                var successResponse = ApiResponse<string>.SuccessResponse(
                    "Tipo de documento eliminado exitosamente", 
                    "Tipo de documento eliminado exitosamente", 
                    StatusCodes.Status200OK);
                return Ok(successResponse);
            }
            catch (InvalidOperationException ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    ex.Message, 
                    StatusCodes.Status409Conflict);
                return Conflict(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<string>.ErrorResponse(
                    "Error interno del servidor", 
                    StatusCodes.Status500InternalServerError, 
                    new List<string> { ex.Message });
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
