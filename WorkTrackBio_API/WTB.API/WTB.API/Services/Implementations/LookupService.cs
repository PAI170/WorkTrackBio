using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WTB.API.Data.Context;
using WTB.API.Services.Interfaces;
using WTB.API.Models.Entities;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de lookups que consulta la base de datos real
    /// </summary>
    public class LookupService : ILookupService
    {
        private readonly WTBDbContext _context;
        private readonly ILogger<LookupService> _logger;

        public LookupService(
            WTBDbContext context,
            ILogger<LookupService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Estados

        public async Task<IEnumerable<States>> GetAllStatesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los estados del sistema");
                
                var states = await _context.States
                    .OrderBy(s => s.StateType)
                    .ThenBy(s => s.StateName)
                    .ToListAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} estados del sistema", states.Count);
                return states;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los estados del sistema");
                throw;
            }
        }

        public async Task<IEnumerable<States>> GetStatesByTypeAsync(string stateType)
        {
            try
            {
                _logger.LogInformation("Obteniendo estados por tipo: {StateType}", stateType);
                
                var states = await _context.States
                    .Where(s => s.StateType.Equals(stateType, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(s => s.StateName)
                    .ToListAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} estados del tipo {StateType}", states.Count, stateType);
                return states;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados por tipo: {StateType}", stateType);
                throw;
            }
        }

        public async Task<States?> GetStateByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo estado con ID: {Id}", id);
                
                var state = await _context.States
                    .FirstOrDefaultAsync(s => s.Id == id);
                
                if (state == null)
                {
                    _logger.LogWarning("No se encontró el estado con ID: {Id}", id);
                }
                else
                {
                    _logger.LogInformation("Estado obtenido exitosamente: {StateName}", state.StateName);
                }
                
                return state;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estado con ID: {Id}", id);
                throw;
            }
        }

        #endregion

        #region Roles

        public async Task<IEnumerable<Roles>> GetAllRolesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los roles del sistema");
                
                var roles = await _context.Roles
                    .OrderBy(r => r.RoleName)
                    .ToListAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} roles del sistema", roles.Count);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles del sistema");
                throw;
            }
        }

        public async Task<Roles?> GetRoleByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo rol con ID: {Id}", id);
                
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == id);
                
                if (role == null)
                {
                    _logger.LogWarning("No se encontró el rol con ID: {Id}", id);
                }
                else
                {
                    _logger.LogInformation("Rol obtenido exitosamente: {RoleName}", role.RoleName);
                }
                
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID: {Id}", id);
                throw;
            }
        }

        #endregion

        #region Tipos de Documento

        public async Task<IEnumerable<DocumentType>> GetAllDocumentTypesAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los tipos de documento");
                
                var documentTypes = await _context.DocumentType
                    .OrderBy(dt => dt.DocumentName)
                    .ToListAsync();
                
                _logger.LogInformation("Se obtuvieron {Count} tipos de documento", documentTypes.Count);
                return documentTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tipos de documento");
                throw;
            }
        }

        public async Task<DocumentType?> GetDocumentTypeByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo tipo de documento con ID: {Id}", id);
                
                var documentType = await _context.DocumentType
                    .FirstOrDefaultAsync(dt => dt.Id == id);
                
                if (documentType == null)
                {
                    _logger.LogWarning("No se encontró el tipo de documento con ID: {Id}", id);
                }
                else
                {
                    _logger.LogInformation("Tipo de documento obtenido exitosamente: {DocumentName}", documentType.DocumentName);
                }
                
                return documentType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipo de documento con ID: {Id}", id);
                throw;
            }
        }

        #endregion

        #region Operaciones Combinadas

        public async Task<LookupData> GetAllLookupsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los lookups del sistema");
                
                var states = await GetAllStatesAsync();
                var roles = await GetAllRolesAsync();
                var documentTypes = await GetAllDocumentTypesAsync();
                
                var lookupData = new LookupData
                {
                    States = states,
                    Roles = roles,
                    DocumentTypes = documentTypes
                };
                
                _logger.LogInformation("Se obtuvieron todos los lookups del sistema: {StatesCount} estados, {RolesCount} roles, {DocumentTypesCount} tipos de documento", 
                    states.Count(), roles.Count(), documentTypes.Count());
                
                return lookupData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los lookups del sistema");
                throw;
            }
        }

        #endregion
    }
}
