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
            var response = await _documentTypeService.GetAllDocumentTypesAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> GetDocumentTypeById(int id)
        {
            var response = await _documentTypeService.GetDocumentTypeByIdAsync(id);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("name/{documentName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> GetDocumentTypeByName(string documentName)
        {
            var response = await _documentTypeService.GetDocumentTypeByNameAsync(documentName);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> DocumentTypeExists(int id)
        {
            var response = await _documentTypeService.DocumentTypeExistsAsync(id);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("name/{documentName}/exists")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> DocumentTypeNameExists(string documentName)
        {
            var response = await _documentTypeService.DocumentTypeNameExistsAsync(documentName);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> CreateDocumentType(CreateDocumentTypeDataTransferObject createDto)
        {
            var validation = await _documentTypeValidator.ValidateCreateAsync(createDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("Datos de tipo de documento inválidos", 400, errors));
            }

            var response = await _documentTypeService.CreateDocumentTypeAsync(createDto);

            if (!response.Success)
            {
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetDocumentTypeById), new { id = response.Data!.Id }, response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> UpdateDocumentType(UpdateDocumentTypeDataTransferObject updateDto)
        {
            var validation = await _documentTypeValidator.ValidateUpdateAsync(updateDto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<DocumentTypeDataTransferObject>.ErrorResponse("Datos de tipo de documento inválidos", 400, errors));
            }

            var response = await _documentTypeService.UpdateDocumentTypeAsync(updateDto);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDocumentType(int id)
        {
            var response = await _documentTypeService.DeleteDocumentTypeAsync(id);

            if (!response.Success)
            {
                if (response.StatusCode == 404) return NotFound(response);
                if (response.StatusCode == 409) return Conflict(response);
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}