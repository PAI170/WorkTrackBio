using Mapster;
using Microsoft.EntityFrameworkCore;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Catalog;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly AppDbContext _context;

        public DocumentTypeService(AppDbContext context)
        {
            _context = context;
        }

        // ─── GET ALL ──────────────────────────────────────────────────────────
        public async Task<IEnumerable<DocumentTypeResponseDto>> GetAllAsync()
        {
            return await _context.DocumentTypes
                .AsNoTracking()
                .Select(d => new DocumentTypeResponseDto
                {
                    Id = d.Id,
                    DocumentName = d.DocumentName,
                    Description = d.Description,
                    EmployeeCount = d.EmployeeInfos.Count(),
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();
        }

        // ─── GET BY ID ────────────────────────────────────────────────────────
        public async Task<DocumentTypeResponseDto> GetByIdAsync(int id)
        {
            var documentType = await _context.DocumentTypes
                .AsNoTracking()
                .Include(d => d.EmployeeInfos)
                .FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new NotFoundException("Tipo de documento no encontrado.");

            var dto = documentType.Adapt<DocumentTypeResponseDto>();
            dto.EmployeeCount = documentType.EmployeeInfos.Count;

            return dto;
        }

        // ─── CREATE ───────────────────────────────────────────────────────────
        public async Task<DocumentTypeResponseDto> CreateAsync(DocumentTypeCreateDto dto)
        {
            var nameExists = await _context.DocumentTypes
                .AnyAsync(d => EF.Functions.Collate(d.DocumentName, "SQL_Latin1_General_CP1_CI_AI") == dto.DocumentName);

            if (nameExists)
                throw new ConflictException("Ya existe un tipo de documento con ese nombre.");

            var documentType = dto.Adapt<DocumentType>();

            _context.DocumentTypes.Add(documentType);
            await _context.SaveChangesAsync();

            var responseDto = documentType.Adapt<DocumentTypeResponseDto>();
            responseDto.EmployeeCount = 0;

            return responseDto;
        }

        // ─── UPDATE ───────────────────────────────────────────────────────────
        public async Task<DocumentTypeResponseDto> UpdateAsync(int id, DocumentTypeUpdateDto dto)
        {
            var documentType = await _context.DocumentTypes
                .Include(d => d.EmployeeInfos)
                .FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new NotFoundException("Tipo de documento no encontrado.");

            var nameExists = await _context.DocumentTypes
                .AnyAsync(d => EF.Functions.Collate(d.DocumentName, "SQL_Latin1_General_CP1_CI_AI") == dto.DocumentName && d.Id != id);

            if (nameExists)
                throw new ConflictException("Ya existe un tipo de documento con ese nombre.");

            dto.Adapt(documentType);
            await _context.SaveChangesAsync();

            var responseDto = documentType.Adapt<DocumentTypeResponseDto>();
            responseDto.EmployeeCount = documentType.EmployeeInfos.Count;

            return responseDto;
        }

        // ─── DELETE ───────────────────────────────────────────────────────────
        public async Task DeleteAsync(int id)
        {
            var documentType = await _context.DocumentTypes
                .Include(d => d.EmployeeInfos)
                .FirstOrDefaultAsync(d => d.Id == id)
                ?? throw new NotFoundException("Tipo de documento no encontrado.");

            if (documentType.EmployeeInfos.Any())
                throw new ConflictException(
                    "No se puede eliminar el tipo de documento porque tiene empleados asignados.");

            _context.DocumentTypes.Remove(documentType);
            await _context.SaveChangesAsync();
        }
    }
}