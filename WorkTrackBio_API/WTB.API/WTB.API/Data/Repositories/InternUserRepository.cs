using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WTB.API.Data.Context;
using WTB.API.Models.Entities;
using WTB.API.Models.DTOs.InternUser;
using WTB.API.Helpers;

namespace WTB.API.Data.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios internos
    /// </summary>
    public class InternUserRepository : Repository<InternUsers>, IInternUserRepository
    {
        private readonly ILogger<InternUserRepository> _logger;

        public InternUserRepository(WTBDbContext context, ILogger<InternUserRepository> logger) 
            : base(context)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtiene usuarios internos con filtros aplicados
        /// </summary>
        public async Task<PagedResponse<InternUsers>> GetFilteredAsync(InternUserFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos filtrados con: {Filter}", 
                    System.Text.Json.JsonSerializer.Serialize(filter));

                var query = _context.InternUsers
                    .Include(u => u.Role)
                    .Include(u => u.State)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(u => 
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(filter.Email))
                {
                    query = query.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(filter.FirstName))
                {
                    query = query.Where(u => u.FirstName.ToLower().Contains(filter.FirstName.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(filter.LastName))
                {
                    query = query.Where(u => u.LastName.ToLower().Contains(filter.LastName.ToLower()));
                }

                if (filter.RolId.HasValue)
                {
                    query = query.Where(u => u.RolId == filter.RolId.Value);
                }

                if (filter.StateId.HasValue)
                {
                    query = query.Where(u => u.StateId == filter.StateId.Value);
                }

                if (filter.CreatedFrom.HasValue)
                {
                    query = query.Where(u => u.CreatedDate >= filter.CreatedFrom.Value);
                }

                if (filter.CreatedTo.HasValue)
                {
                    query = query.Where(u => u.CreatedDate <= filter.CreatedTo.Value);
                }

                if (filter.LastLoginFrom.HasValue)
                {
                    query = query.Where(u => u.LastLogin >= filter.LastLoginFrom.Value);
                }

                if (filter.LastLoginTo.HasValue)
                {
                    query = query.Where(u => u.LastLogin <= filter.LastLoginTo.Value);
                }

                // Aplicar ordenamiento
                query = filter.SortBy?.ToLower() switch
                {
                    "email" => filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                    "firstname" => filter.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
                    "lastname" => filter.SortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
                    "creationdate" => filter.SortDescending ? query.OrderByDescending(u => u.CreatedDate) : query.OrderBy(u => u.CreatedDate),
                    "lastlogin" => filter.SortDescending ? query.OrderByDescending(u => u.LastLogin) : query.OrderBy(u => u.LastLogin),
                    _ => filter.SortDescending ? query.OrderByDescending(u => u.CreatedDate) : query.OrderBy(u => u.CreatedDate)
                };

                // Contar total de registros
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var items = await query
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                _logger.LogInformation("Usuarios internos obtenidos: {Count} de {Total}", items.Count, totalCount);

                return PagedResponse<InternUsers>.Create(items, filter.Page, filter.PageSize, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos filtrados");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por ID con todas sus relaciones
        /// </summary>
        public async Task<InternUsers?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuario interno con ID: {Id}", id);

                var user = await _context.InternUsers
                    .Include(u => u.Role)
                    .Include(u => u.State)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogWarning("Usuario interno con ID {Id} no encontrado", id);
                }
                else
                {
                    _logger.LogInformation("Usuario interno obtenido: {Email}", user.Email);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario interno con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por email
        /// </summary>
        public async Task<InternUsers?> GetByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuario interno por email: {Email}", email);

                var user = await _context.InternUsers
                    .Include(u => u.Role)
                    .Include(u => u.State)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    _logger.LogWarning("Usuario interno con email {Email} no encontrado", email);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario interno por email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Verifica si existe un email
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            try
            {
                var query = _context.InternUsers.Where(u => u.Email == email);
                
                if (excludeId.HasValue)
                {
                    query = query.Where(u => u.Id != excludeId.Value);
                }

                var exists = await query.AnyAsync();
                _logger.LogInformation("Verificación de email '{Email}' (excluyendo ID {ExcludeId}): {Exists}", 
                    email, excludeId, exists);

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando existencia de email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Busca usuarios internos por término de búsqueda
        /// </summary>
        public async Task<IEnumerable<InternUsers>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando usuarios internos con término: {SearchTerm}", searchTerm);

                var users = await _context.InternUsers
                    .Include(u => u.Role)
                    .Include(u => u.State)
                    .Where(u => 
                        u.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                        u.LastName.ToLower().Contains(searchTerm.ToLower()) ||
                        u.Email.ToLower().Contains(searchTerm.ToLower()))
                    .OrderBy(u => u.Email)
                    .ToListAsync();

                _logger.LogInformation("Búsqueda completada: {Count} usuarios encontrados", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buscando usuarios internos con término: {SearchTerm}", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// Obtiene usuarios internos activos
        /// </summary>
        public async Task<IEnumerable<InternUsers>> GetActiveAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos activos");

                var users = await _context.InternUsers
                    .Include(u => u.Role)
                    .Include(u => u.State)
                    .Where(u => u.State.StateType == "Active" || u.State.StateName == "Activo") // Filtrar por tipo o nombre de estado
                    .OrderBy(u => u.Email)
                    .ToListAsync();

                _logger.LogInformation("Usuarios activos obtenidos: {Count}", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos activos");
                throw;
            }
        }

        /// <summary>
        /// Obtiene usuarios internos por rol
        /// </summary>
        public async Task<IEnumerable<InternUsers>> GetByRoleAsync(int rolId)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos por rol ID: {RolId}", rolId);

                var users = await _context.InternUsers
                    .Include(u => u.Role)
                    .Include(u => u.State)
                    .Where(u => u.RolId == rolId)
                    .OrderBy(u => u.Email)
                    .ToListAsync();

                _logger.LogInformation("Usuarios por rol obtenidos: {Count}", users.Count);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos por rol ID: {RolId}", rolId);
                throw;
            }
        }
    }
}
