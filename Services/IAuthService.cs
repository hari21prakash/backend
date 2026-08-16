using Backend.DTOs;

namespace Backend.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<UserResponseDto?> GetByIdAsync(Guid id);
    }

    public class AuthException : Exception
    {
        public AuthException(string message) : base(message) { }
    }
}
