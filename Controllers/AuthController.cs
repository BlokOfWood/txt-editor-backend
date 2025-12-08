using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models.Dtos;
using aresu_txt_editor_backend.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace aresu_txt_editor_backend.Controllers;

[ApiController]
[Route("user")]
public class AuthController(ILogger<AuthController> _logger, IUserService _userService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        var result = _userService.TryLoginUser(loginDto.Username, loginDto.Password, out var userId);
        
        // Provide JWT token on successful login
        if (result == LoginResult.Success)
        {
             //var token = Jwt
        }

        return result switch
        {
            LoginResult.Success => Ok(new { UserId = userId, Message = "Login successful" }),
            LoginResult.UserNotFound or LoginResult.InvalidCredentials =>
                Unauthorized("Invalid username or password"),
            _ => StatusCode(500, "An unknown error occurred")
        };
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto registerDto)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.Log(LogLevel.Debug, "Registering user {username}.", registerDto.Username);
        }

        var result = _userService.RegisterUser(registerDto);

        return result switch
        {
            RegisterResult.Success =>
                Ok("User registered successfully"),
            RegisterResult.UsernameAlreadyExists =>
                Conflict("Username already exists"),
            _ =>
                StatusCode(500, "An unknown error occurred"),
        };
    }
}