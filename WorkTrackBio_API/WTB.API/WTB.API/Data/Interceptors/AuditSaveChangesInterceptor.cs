using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using WTB.API.Models.Entities;
using WTB.API.Models.Configuration;
using WTB.API.Models.DTOs.Common;
using WTB.API.Data.Context;

namespace WTB.API.Data.Interceptors
{
    /// <summary>
    /// Interceptor que captura automáticamente todos los cambios para auditoría
    /// </summary>
    public class AuditSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SystemSettings _systemSettings;

        public AuditSaveChangesInterceptor(
            IHttpContextAccessor httpContextAccessor,
            IOptions<SystemSettings> systemSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _systemSettings = systemSettings.Value;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context != null)
            {
                var auditEntries = OnBeforeSaveChanges(context);
                context.Set<AuditRegister>().AddRange(auditEntries);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private List<AuditRegister> OnBeforeSaveChanges(DbContext context)
        {
            var auditEntries = new List<AuditRegister>();
            var entries = context.ChangeTracker.Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = CreateAuditEntry(entry);
                if (auditEntry != null)
                {
                    auditEntries.Add(auditEntry);
                }
            }

            return auditEntries;
        }

        private AuditRegister? CreateAuditEntry(EntityEntry<AuditableEntity> entry)
        {
            var entity = entry.Entity;
            var entityType = entity.GetEntityType();
            var entityId = entity.GetEntityId();
            var adminId = GetCurrentUserId();

            // Determinar el tipo de acción
            var actionType = entry.State switch
            {
                EntityState.Added => "CREATE",
                EntityState.Modified => "UPDATE",
                EntityState.Deleted => "DELETE",
                _ => "UNKNOWN"
            };

            // Crear el registro de auditoría base
            var auditEntry = new AuditRegister
            {
                EntityType = entityType,
                EntityId = entityId,
                ActionType = actionType,
                ActionDate = DateTime.UtcNow,
                AdminId = adminId,
                DetailChange = GenerateDetailChange(entry, actionType)
            };

            // Si es una modificación, capturar cambios específicos
            if (entry.State == EntityState.Modified)
            {
                var propertyChanges = GetPropertyChanges(entry);
                if (propertyChanges.Any())
                {
                    // Para múltiples cambios, crear un registro por propiedad
                    var auditEntries = new List<AuditRegister>();
                    
                    foreach (var change in propertyChanges)
                    {
                        auditEntries.Add(new AuditRegister
                        {
                            EntityType = entityType,
                            EntityId = entityId,
                            ActionType = "UPDATE",
                            PropertyName = change.PropertyName,
                            OldValue = change.OldValue,
                            NewValue = change.NewValue,
                            ActionDate = DateTime.UtcNow,
                            AdminId = adminId,
                            DetailChange = $"Campo '{change.PropertyName}' cambió de '{change.OldValue}' a '{change.NewValue}'"
                        });
                    }

                    // Retornar solo el primer cambio (el resto se agregará en el loop)
                    return auditEntries.First();
                }
            }

            return auditEntry;
        }

        private List<PropertyChange> GetPropertyChanges(EntityEntry<AuditableEntity> entry)
        {
            var changes = new List<PropertyChange>();

            foreach (var property in entry.Properties)
            {
                if (property.IsModified)
                {
                    var oldValue = property.OriginalValue?.ToString() ?? "null";
                    var newValue = property.CurrentValue?.ToString() ?? "null";

                    // Solo registrar cambios si los valores son diferentes
                    if (oldValue != newValue)
                    {
                        changes.Add(new PropertyChange
                        {
                            PropertyName = property.Metadata.Name,
                            OldValue = oldValue,
                            NewValue = newValue
                        });
                    }
                }
            }

            return changes;
        }

        private string GenerateDetailChange(EntityEntry<AuditableEntity> entry, string actionType)
        {
            var entityName = entry.Entity.GetEntityType();
            
            return actionType switch
            {
                "CREATE" => $"Nuevo {entityName} creado",
                "UPDATE" => $"{entityName} modificado",
                "DELETE" => $"{entityName} eliminado",
                _ => $"Operación {actionType} en {entityName}"
            };
        }

        private int GetCurrentUserId()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    // Si no hay contexto HTTP (ejecución en background, migraciones, etc.)
                    return GetSystemUserId();
                }

                // Intentar obtener el usuario del claim de autenticación
                var user = httpContext.User;
                if (user?.Identity?.IsAuthenticated == true)
                {
                    // Buscar el claim con el ID del usuario
                    var userIdClaim = user.FindFirst("UserId") ?? 
                                    user.FindFirst("sub") ?? 
                                    user.FindFirst("nameidentifier");
                    
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }

                    // Si no hay claim de ID, buscar por email
                    var emailClaim = user.FindFirst("Email") ?? 
                                   user.FindFirst("preferred_username") ?? 
                                   user.FindFirst("name");
                    
                    if (emailClaim != null)
                    {
                        return GetUserIdByEmail(emailClaim.Value);
                    }
                }

                // Si no se puede obtener el usuario del contexto, usar el sistema
                return GetSystemUserId();
            }
            catch (Exception)
            {
                // En caso de error, usar usuario del sistema
                return GetSystemUserId();
            }
        }

        /// <summary>
        /// Obtiene el ID del usuario del sistema para operaciones automáticas
        /// </summary>
        private int GetSystemUserId()
        {
            return _systemSettings.SystemUserId;
        }

        /// <summary>
        /// Obtiene el ID del usuario por email
        /// </summary>
        private int GetUserIdByEmail(string email)
        {
            try
            {
                // Intentar obtener el contexto de base de datos
                var context = _httpContextAccessor.HttpContext?.RequestServices
                    .GetService<WTBDbContext>();
                
                if (context != null)
                {
                    var user = context.InternUsers
                        .Where(u => u.Email == email)
                        .Select(u => u.Id)
                        .FirstOrDefault();
                    
                    if (user > 0)
                        return user;
                }
            }
            catch (Exception)
            {
                // Si hay error, continuar con usuario del sistema
            }

            return GetSystemUserId();
        }
    }

    /// <summary>
    /// Clase auxiliar para representar cambios en propiedades
    /// </summary>
    public class PropertyChange
    {
        public string PropertyName { get; set; } = string.Empty;
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
    }
}
