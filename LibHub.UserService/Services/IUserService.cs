using LibHub.UserService.DTOs;

namespace LibHub.UserService.Services;

public interface IUserService 
{
    Task<UserDto?> RegisterUserAsync(RegisterRequest request);
    Task<(bool IsSuccess, string Token)> AuthenticateUserAsync(LoginRequest request);
    Task<UserDto?> GetUserProfileAsync(Guid id);
}
