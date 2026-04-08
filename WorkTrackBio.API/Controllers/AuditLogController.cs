using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.DataTransferObjects.Audit;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuditLogController : ControllerBase
    {
        // ─── Campos privados ──────────────────────────────────────────────────
        private readonly IAuditLogService _auditLogService;

        // ─── Constructor ──────────────────────────────────────────────────────
        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? tableName,
            [FromQuery] string? action,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            var data = await _auditLogService.GetAllAsync(tableName, action, from, to);
            return Ok(ApiResponse<IEnumerable<AuditLogResponseDto>>
                .SuccessResponse(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _auditLogService.GetByIdAsync(id);
            return Ok(ApiResponse<AuditLogResponseDto>
                .SuccessResponse(data));
        }
    }
}