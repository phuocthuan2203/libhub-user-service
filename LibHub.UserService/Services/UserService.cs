using LibHub.UserService.DTOs;
using LibHub.UserService.Data.Repositories;
using LibHub.UserService.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace LibHub.UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> RegisterUserAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return null;
        }

        var salt = GenerateSalt();
        var hashedPassword = HashPassword(request.Password, salt);

        var user = User.Create(request.Name, request.Email, "User");
        user.SetPassword(hashedPassword, salt);

        var savedUser = await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = savedUser.Id,
            Name = savedUser.Name,
            Email = savedUser.Email,
            Role = savedUser.Role
        };
    }

    public async Task<(bool IsSuccess, string Token)> AuthenticateUserAsync(LoginRequest request)
    {
        return (false, string.Empty);
    }

    public async Task<UserDto?> GetUserProfileAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }

    private static string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    private static string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
        
        Array.Copy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
        Array.Copy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(combinedBytes);
        return Convert.ToBase64String(hashedBytes);
    }
}
