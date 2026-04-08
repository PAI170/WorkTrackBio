using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.DataTransferObjects.Audit;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuditLogResponseDto>> GetAllAsync(
            string? tableName, string? action, DateTime? from, DateTime? to)
        {
            var query = _context.SystemAuditLogs
                .AsNoTracking()
                .Include(a => a.AppUser!)
                    .ThenInclude(u => u.EmployeeInfo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tableName))
                query = query.Where(a => a.TableName == tableName);

            if (!string.IsNullOrWhiteSpace(action))
                query = query.Where(a => a.Action == action);

            if (from.HasValue)
                query = query.Where(a => a.Timestamp >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.Timestamp <= to.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new AuditLogResponseDto
                {
                    Id = a.Id,
                    PerformedBy = a.AppUser!.EmployeeInfo.FirstName + " " + a.AppUser.EmployeeInfo.LastName,
                    TableName = a.TableName,
                    Action = a.Action,
                    RecordId = a.RecordId,
                    OldValues = a.OldValues,
                    NewValues = a.NewValues,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();
        }

        public async Task<AuditLogResponseDto> GetByIdAsync(int id)
        {
            var log = await _context.SystemAuditLogs
                .AsNoTracking()
                .Include(a => a.AppUser!)
                    .ThenInclude(u => u.EmployeeInfo)
                .FirstOrDefaultAsync(a => a.Id == id)
                ?? throw new NotFoundException("Registro de auditoría no encontrado.");

            return new AuditLogResponseDto
            {
                Id = log.Id,
                PerformedBy = $"{log.AppUser!.EmployeeInfo.FirstName} {log.AppUser.EmployeeInfo.LastName}",
                TableName = log.TableName,
                Action = log.Action,
                RecordId = log.RecordId,
                OldValues = log.OldValues,
                NewValues = log.NewValues,
                Timestamp = log.Timestamp
            };
        }
    }
}