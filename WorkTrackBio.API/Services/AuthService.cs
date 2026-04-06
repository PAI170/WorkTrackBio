using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WorkTrackBio.API.Exceptions;
using WorkTrackBio.API.Data;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Auth;
using WorkTrackBio.API.Interfaces;

namespace WorkTrackBio.API.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // 1. buscar el usuario por email
            var user = await _context.AppUsers
                .Include(u => u.Role)
                .Include(u => u.EmployeeInfo)
                .FirstOrDefaultAsync(u => u.WorkEmail == request.WorkEmail)
                ?? throw new UnauthorizedException("Credenciales incorrectas.");

            // 2. verificar si está bloqueado
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                throw new UnauthorizedException(
                    "Cuenta bloqueada temporalmente por múltiples intentos fallidos. Intente más tarde.");

            // 3. verificar si está activo
            if (!user.IsActive)
                throw new UnauthorizedException("Cuenta inactiva.");

            // 4. verificar la contraseña
            if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 3)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                await _context.SaveChangesAsync();

                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                    throw new UnauthorizedException(
                        "Cuenta bloqueada temporalmente por múltiples intentos fallidos. Intente más tarde.");

                throw new UnauthorizedException("Credenciales incorrectas.");
            }

            // 5. resetear intentos fallidos
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLogin = DateTime.UtcNow;

            // 6. generar tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var refreshExpiration = DateTime.UtcNow.AddDays(
                int.Parse(jwtSettings["RefreshTokenExpirationDays"]!));

            // 7. guardar sesión
            var session = new UserSession
            {
                AppUserId = user.Id,
                Token = refreshToken,
                JwtId = Guid.NewGuid().ToString(),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = refreshExpiration
            };

            _context.UserSessions.Add(session);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                RefreshTokenExpiration = refreshExpiration,
                User = new UserInfoDto
                {
                    Id = user.Id,
                    FullName = $"{user.EmployeeInfo.FirstName} {user.EmployeeInfo.LastName}",
                    WorkEmail = user.WorkEmail,
                    Role = user.Role.RoleName
                }
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // 1. buscar la sesión
            var session = await _context.UserSessions
                .Include(s => s.AppUser!)
                    .ThenInclude(u => u.Role)
                .Include(s => s.AppUser!)
                    .ThenInclude(u => u.EmployeeInfo)
                .FirstOrDefaultAsync(s => s.Token == refreshToken
                                       && !s.IsRevoked
                                       && !s.IsUsed
                                       && s.ExpiryDate > DateTime.UtcNow)
                ?? throw new UnauthorizedException("Refresh token inválido o expirado.");

            // 2. marcar sesión anterior como usada
            session.IsUsed = true;

            // 3. generar nuevos tokens
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var newAccessToken = GenerateAccessToken(session.AppUser!);
            var newRefreshToken = GenerateRefreshToken();
            var refreshExpiration = DateTime.UtcNow.AddDays(
                int.Parse(jwtSettings["RefreshTokenExpirationDays"]!));

            // 4. guardar nueva sesión
            var newSession = new UserSession
            {
                AppUserId = session.AppUserId,
                Token = newRefreshToken,
                JwtId = Guid.NewGuid().ToString(),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = refreshExpiration
            };

            _context.UserSessions.Add(newSession);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                RefreshTokenExpiration = refreshExpiration,
                User = new UserInfoDto
                {
                    Id = session.AppUser!.Id,
                    FullName = $"{session.AppUser.EmployeeInfo.FirstName} {session.AppUser.EmployeeInfo.LastName}",
                    WorkEmail = session.AppUser.WorkEmail,
                    Role = session.AppUser.Role.RoleName
                }
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Token == refreshToken)
                ?? throw new UnauthorizedException("Refresh token inválido.");

            session.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        // ── Métodos privados ──────────────────────────────────────────────────

        private string GenerateAccessToken(AppUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.WorkEmail),
                new Claim(ClaimTypes.Role, user.Role.RoleName),
                new Claim("EmployeeInfoId", user.EmployeeInfoId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(jwtSettings["AccessTokenExpirationMinutes"]!)),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static bool VerifyPassword(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromHexString(salt);
            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                saltBytes,
                100_000,
                HashAlgorithmName.SHA512);
            var hashBytes = pbkdf2.GetBytes(64);
            return string.Equals(Convert.ToHexString(hashBytes), hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
