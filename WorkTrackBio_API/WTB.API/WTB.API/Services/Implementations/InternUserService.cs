using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Models.DTOs.InternUser;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;
using WTB.API.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de usuarios internos
    /// </summary>
    public class InternUserService : IInternUserService
    {
        private readonly IInternUserRepository _internUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InternUserService> _logger;

        public InternUserService(
            IInternUserRepository internUserRepository,
            IUnitOfWork unitOfWork,
            ILogger<InternUserService> logger)
        {
            _internUserRepository = internUserRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene usuarios internos con filtros aplicados
        /// </summary>
        public async Task<PagedResponse<InternUserResponseDto>> GetFilteredAsync(InternUserFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos filtrados");

                var pagedResult = await _internUserRepository.GetFilteredAsync(filter);

                var responseDtos = pagedResult.Data?.Select(MapToResponseDto) ?? Enumerable.Empty<InternUserResponseDto>();

                return PagedResponse<InternUserResponseDto>.Create(responseDtos, pagedResult.Page, pagedResult.PageSize, pagedResult.TotalRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos filtrados");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un usuario interno por ID
        /// </summary>
        public async Task<InternUserDetailsDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuario interno con ID: {Id}", id);

                var user = await _internUserRepository.GetByIdWithIncludesAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario interno con ID {Id} no encontrado", id);
                    return null;
                }

                var detailsDto = MapToDetailsDto(user);
                _logger.LogInformation("Usuario interno obtenido: {Email}", user.Email);

                return detailsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario interno con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Crea un nuevo usuario interno
        /// </summary>
        public async Task<InternUserResponseDto> CreateAsync(CreateInternUserDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando usuario interno: {Email}", createDto.Email);

                // Validar datos de entrada
                var (isValid, errorMessage) = await ValidateForCreationAsync(createDto);
                if (!isValid)
                {
                    _logger.LogWarning("Validación fallida para crear usuario interno {Email}: {Error}", 
                        createDto.Email, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Crear entidad
                var user = new InternUsers
                {
                    Email = createDto.Email,
                    FirstName = createDto.FirstName,
                    LastName = createDto.LastName,
                    RolId = createDto.RolId,
                    StateId = 1, // Estado activo por defecto
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                // Hash de la contraseña
                user.PasswordHash = HashPassword(createDto.Password);
                user.PasswordSalt = GenerateSalt();

                // Guardar en base de datos
                await _internUserRepository.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Usuario interno creado exitosamente: {Email} con ID {Id}", 
                    user.Email, user.Id);

                return MapToResponseDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando usuario interno: {Email}", createDto.Email);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un usuario interno existente
        /// </summary>
        public async Task<InternUserResponseDto> UpdateAsync(int id, UpdateInternUserDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando usuario interno con ID: {Id}", id);

                // Validar datos de entrada
                var (isValid, errorMessage) = await ValidateForUpdateAsync(id, updateDto);
                if (!isValid)
                {
                    _logger.LogWarning("Validación fallida para actualizar usuario interno {Id}: {Error}", 
                        id, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Obtener usuario existente
                var existingUser = await _internUserRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Usuario interno con ID {Id} no encontrado para actualizar", id);
                    throw new InvalidOperationException($"Usuario interno con ID {id} no encontrado");
                }

                // Actualizar propiedades
                existingUser.FirstName = updateDto.FirstName;
                existingUser.LastName = updateDto.LastName;
                existingUser.Email = updateDto.Email;
                existingUser.RolId = updateDto.RolId;
                existingUser.StateId = updateDto.StateId;
                existingUser.ModifiedDate = DateTime.Now;

                // Actualizar en base de datos
                await _internUserRepository.UpdateAsync(existingUser);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Usuario interno actualizado exitosamente: {Email}", existingUser.Email);

                return MapToResponseDto(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando usuario interno con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Elimina un usuario interno (soft delete)
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando usuario interno con ID: {Id}", id);

                // Validar si se puede eliminar
                var (canDelete, errorMessage) = await ValidateForDeletionAsync(id);
                if (!canDelete)
                {
                    _logger.LogWarning("No se puede eliminar usuario interno {Id}: {Error}", id, errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Obtener usuario
                var user = await _internUserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario interno con ID {Id} no encontrado para eliminar", id);
                    throw new InvalidOperationException($"Usuario interno con ID {id} no encontrado");
                }

                // Soft delete - cambiar estado a inactivo
                user.StateId = 2; // Estado inactivo
                user.ModifiedDate = DateTime.Now;

                await _internUserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Usuario interno eliminado exitosamente: {Email}", user.Email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando usuario interno con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Busca usuarios internos por término de búsqueda
        /// </summary>
        public async Task<IEnumerable<InternUserListDto>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando usuarios internos con término: {SearchTerm}", searchTerm);

                var users = await _internUserRepository.SearchAsync(searchTerm);
                var listDtos = users.Select(MapToListDto);

                _logger.LogInformation("Búsqueda completada: {Count} usuarios encontrados", listDtos.Count());

                return listDtos;
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
        public async Task<IEnumerable<InternUserListDto>> GetActiveAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos activos");

                var users = await _internUserRepository.GetActiveAsync();
                var listDtos = users.Select(MapToListDto);

                _logger.LogInformation("Usuarios activos obtenidos: {Count}", listDtos.Count());

                return listDtos;
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
        public async Task<IEnumerable<InternUserListDto>> GetByRoleAsync(int rolId)
        {
            try
            {
                _logger.LogInformation("Obteniendo usuarios internos por rol ID: {RolId}", rolId);

                var users = await _internUserRepository.GetByRoleAsync(rolId);
                var listDtos = users.Select(MapToListDto);

                _logger.LogInformation("Usuarios por rol obtenidos: {Count}", listDtos.Count());

                return listDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuarios internos por rol ID: {RolId}", rolId);
                throw;
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario interno
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
        {
            try
            {
                _logger.LogInformation("Cambiando contraseña para usuario interno con ID: {Id}", id);

                // Obtener usuario
                var user = await _internUserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Usuario interno con ID {Id} no encontrado para cambiar contraseña", id);
                    throw new InvalidOperationException($"Usuario interno con ID {id} no encontrado");
                }

                // Verificar contraseña actual
                var currentPasswordHash = HashPassword(changePasswordDto.CurrentPassword);
                if (user.PasswordHash != currentPasswordHash)
                {
                    _logger.LogWarning("Contraseña actual incorrecta para usuario {Email}", user.Email);
                    throw new InvalidOperationException("Contraseña actual incorrecta");
                }

                // Hash de la nueva contraseña
                user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                user.PasswordSalt = GenerateSalt();
                user.ModifiedDate = DateTime.Now;

                // Actualizar en base de datos
                await _internUserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {Email}", user.Email);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cambiando contraseña para usuario interno con ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Valida los datos para crear un usuario interno
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage)> ValidateForCreationAsync(CreateInternUserDto createDto)
        {
            try
            {
                // Verificar si el email ya existe
                if (await _internUserRepository.EmailExistsAsync(createDto.Email))
                {
                    return (false, $"El email '{createDto.Email}' ya está en uso");
                }

                // Verificar que las contraseñas coincidan
                if (createDto.Password != createDto.ConfirmPassword)
                {
                    return (false, "Las contraseñas no coinciden");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para crear usuario interno: {Email}", createDto.Email);
                return (false, "Error interno durante la validación");
            }
        }

        /// <summary>
        /// Valida los datos para actualizar un usuario interno
        /// </summary>
        public async Task<(bool IsValid, string? ErrorMessage)> ValidateForUpdateAsync(int id, UpdateInternUserDto updateDto)
        {
            try
            {
                // Verificar si el email ya existe (excluyendo el usuario actual)
                if (await _internUserRepository.EmailExistsAsync(updateDto.Email, id))
                {
                    return (false, $"El email '{updateDto.Email}' ya está en uso por otro usuario");
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando datos para actualizar usuario interno con ID: {Id}", id);
                return (false, "Error interno durante la validación");
            }
        }

        /// <summary>
        /// Valida si se puede eliminar un usuario interno
        /// </summary>
        public Task<(bool CanDelete, string? ErrorMessage)> ValidateForDeletionAsync(int id)
        {
            try
            {
                // Por ahora permitimos eliminar cualquier usuario
                // En el futuro se pueden agregar validaciones adicionales
                // como verificar si tiene registros relacionados
                return Task.FromResult((true, (string?)null));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validando si se puede eliminar usuario interno con ID: {Id}", id);
                return Task.FromResult((false, (string?)"Error interno durante la validación"));
            }
        }

        #region Private Methods

        /// <summary>
        /// Mapea entidad a DTO de respuesta
        /// </summary>
        private static InternUserResponseDto MapToResponseDto(InternUsers user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var daysSinceCreation = (DateTime.Now - user.CreatedDate).Days;
            var daysSinceLastLogin = user.LastLogin.HasValue ? (DateTime.Now - user.LastLogin.Value).Days : (int?)null;
            var isActive = user.State?.StateType == "Active" || user.State?.StateName == "Activo";

            return new InternUserResponseDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                fullName,
                user.RolId,
                user.Role?.RoleName ?? "Rol no encontrado",
                user.StateId,
                user.State?.StateName ?? "Estado no encontrado",
                user.CreatedDate,
                user.LastLogin,
                isActive,
                daysSinceCreation,
                daysSinceLastLogin,
                isActive ? "Activo" : "Inactivo",
                isActive,
                0, // TotalAuditEntries - se puede implementar después
                null // LastAuditAction - se puede implementar después
            );
        }

        /// <summary>
        /// Mapea entidad a DTO de detalles
        /// </summary>
        private static InternUserDetailsDto MapToDetailsDto(InternUsers user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var daysSinceCreation = (DateTime.Now - user.CreatedDate).Days;
            var daysSinceLastLogin = user.LastLogin.HasValue ? (DateTime.Now - user.LastLogin.Value).Days : (int?)null;
            var isActive = user.State?.StateType == "Active" || user.State?.StateName == "Activo";

            // Crear DTOs anidados
            var roleInfo = new RoleInfoDto(
                user.Role?.Id ?? 0,
                user.Role?.RoleName ?? "Rol no encontrado",
                user.Role?.Description,
                false, // HasFullAccess - se puede implementar después
                new List<string>() // Permissions - se puede implementar después
            );

            var stateInfo = new StateInfoDto(
                user.State?.Id ?? 0,
                user.State?.StateName ?? "Estado no encontrado",
                isActive,
                user.State?.Description
            );

            var activityStats = new UserActivityStatsDto(
                0, // TotalAuditEntries
                0, // AuditEntriesThisMonth
                0, // AuditEntriesToday
                null, // LastAuditAction
                null, // LastAuditType
                daysSinceCreation,
                daysSinceLastLogin,
                isActive,
                0.0m // ActivityScore
            );

            var recentAuditActions = new List<UserAuditDto>(); // Se puede implementar después

            return new InternUserDetailsDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                fullName,
                user.RolId,
                user.Role?.RoleName ?? "Rol no encontrado",
                user.StateId,
                user.State?.StateName ?? "Estado no encontrado",
                user.CreatedDate,
                user.LastLogin,
                roleInfo,
                stateInfo,
                activityStats,
                recentAuditActions
            );
        }

        /// <summary>
        /// Mapea entidad a DTO de lista
        /// </summary>
        private static InternUserListDto MapToListDto(InternUsers user)
        {
            var fullName = $"{user.FirstName} {user.LastName}";
            var isActive = user.State?.StateType == "Active" || user.State?.StateName == "Activo";
            var status = isActive ? "Activo" : "Inactivo";

            return new InternUserListDto(
                user.Id,
                user.Email,
                fullName,
                user.Role?.RoleName ?? "Rol no encontrado",
                user.State?.StateName ?? "Estado no encontrado",
                isActive,
                user.CreatedDate,
                user.LastLogin,
                status
            );
        }

        /// <summary>
        /// Genera hash de contraseña usando SHA256
        /// </summary>
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// Genera salt aleatorio para la contraseña
        /// </summary>
        private static string GenerateSalt()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        #endregion
    }
}
