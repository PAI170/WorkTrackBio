using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Catalog;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DocumentTypeController : ControllerBase
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IDocumentTypeService _documentTypeService;

        // ─── Constructor ──────────────────────────────────────────────────────
        public DocumentTypeController(IDocumentTypeService documentTypeService)
        {
            _documentTypeService = documentTypeService;
        }

        // ─── GET api/documenttype ─────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _documentTypeService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<DocumentTypeResponseDto>>
                .SuccessResponse(data));
        }

        // ─── GET api/documenttype/{id} ────────────────────────────────────────
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _documentTypeService.GetByIdAsync(id);
            return Ok(ApiResponse<DocumentTypeResponseDto>
                .SuccessResponse(data));
        }

        // ─── POST api/documenttype ────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DocumentTypeCreateDto dto)
        {
            var data = await _documentTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = data.Id },
                ApiResponse<DocumentTypeResponseDto>
                    .SuccessResponse(data, "Tipo de documento creado exitosamente.", 201));
        }

        // ─── PUT api/documenttype/{id} ────────────────────────────────────────
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DocumentTypeUpdateDto dto)
        {
            var data = await _documentTypeService.UpdateAsync(id, dto);
            return Ok(ApiResponse<DocumentTypeResponseDto>
                .SuccessResponse(data, "Tipo de documento actualizado exitosamente."));
        }

        // ─── DELETE api/documenttype/{id} ─────────────────────────────────────
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _documentTypeService.DeleteAsync(id);
            return Ok(ApiResponse<object>
                .SuccessResponse("Tipo de documento eliminado exitosamente."));
        }
    }
}