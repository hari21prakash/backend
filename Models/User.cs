using System;

namespace Backend.Models
{
    /// <summary>
    /// Application user for login authentication. Passwords are never stored
    /// in plain text — only a salted hash produced by PBKDF2 (see PasswordHasher).
    /// </summary>
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Email { get; set; } = string.Empty;

        // Never store or return the plain password — only the salted hash.
        public string PasswordHash { get; set; } = string.Empty;

        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
