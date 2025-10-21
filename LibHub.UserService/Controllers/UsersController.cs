using LibHub.UserService.DTOs;
using LibHub.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibHub.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _userService.RegisterUserAsync(request);
        
        if (result == null)
        {
            return Conflict(new { message = "User with this email already exists" });
        }

        return CreatedAtAction(nameof(GetProfile), new { id = result.Id }, result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (isSuccess, token) = await _userService.AuthenticateUserAsync(request);
        
        if (!isSuccess)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        return Ok(new { token });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(Guid id)
    {
        var user = await _userService.GetUserProfileAsync(id);
        
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(user);
    }
}
