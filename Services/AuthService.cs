using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            var exists = await _context.Users.AnyAsync(u => u.Email == email);
            if (exists)
                throw new AuthException("An account with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                FullName = string.IsNullOrWhiteSpace(dto.FullName) ? null : dto.FullName.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

            // Deliberately use the same generic message whether the email
            // doesn't exist or the password is wrong, to avoid leaking which one failed.
            if (user is null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
                throw new AuthException("Invalid email or password.");

            return BuildAuthResponse(user);
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return user is null ? null : MapToDto(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured. Set the Jwt__Key environment variable.");
            var issuer = _configuration["Jwt:Issuer"] ?? "ProjectHub";
            var audience = _configuration["Jwt:Audience"] ?? "ProjectHubClient";
            var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var m) ? m : 60 * 24; // 24h default

            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                User = MapToDto(user)
            };
        }

        private static UserResponseDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }
}
