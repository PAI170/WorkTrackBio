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

        public DocumentTypeController(IDocumentTypeService documentTypeService, IDocumentTypeValidator documentTypeValidator)
        {
            _documentTypeService = documentTypeService ?? throw new ArgumentNullException(nameof(documentTypeService));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DocumentTypeDataTransferObject>>>> GetAllDocumentTypes()
        {
            var response = await _documentTypeService.GetAllDocumentTypesAsync();
            if(!response.Success)
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> CreateDocumentType(CreateDocumentTypeDataTransferObject createDto)
        {
            var response = await _documentTypeService.CreateDocumentTypeAsync(createDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
            return CreatedAtAction(nameof(GetDocumentTypeById), new { id = response.Data!.Id }, response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<DocumentTypeDataTransferObject>>> UpdateDocumentType(UpdateDocumentTypeDataTransferObject updateDto)
        {
            var response = await _documentTypeService.UpdateDocumentTypeAsync(updateDto);
            if (!response.Success)
                return StatusCode(response.StatusCode, response);
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
                return StatusCode(response.StatusCode, response);
            return Ok(response);
        }
    }
}