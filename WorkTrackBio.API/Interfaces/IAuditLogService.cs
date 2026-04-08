using WorkTrackBio.API.DataTransferObjects.Audit;

namespace WorkTrackBio.API.Interfaces
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLogResponseDto>> GetAllAsync(string? tableName, string? action, DateTime? from, DateTime? to);
        Task<AuditLogResponseDto> GetByIdAsync(int id);
    }
}