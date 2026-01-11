using System.Security.Claims;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models;
using aresu_txt_editor_backend.Models.Dtos;
using aresu_txt_editor_backend.Models.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace aresu_txt_editor_backend.Controllers;

[ApiController]
[Route("user")]
public class AuthController(
    ILogger<AuthController> _logger,
    IUserService _userService,
    IOptions<AuthConfiguration> authConfig) : ControllerBase
{
    private readonly AuthConfiguration _authConfig = authConfig.Value;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = _userService.TryLoginUser(loginDto.Username, loginDto.Password, out var userId);

        if (result == LoginResult.Success)
        {
            // Create claims for the authenticated user
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, loginDto.Username),
                new(ClaimTypes.NameIdentifier, userId.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Cookie persists across browser sessions
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(_authConfig.AccessTokenExpirationDays)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(new { message = "Login successful", username = loginDto.Username });
        }

        return result switch
        {
            LoginResult.UserNotFound or LoginResult.InvalidCredentials =>
                Unauthorized("Invalid username or password"),
            _ => StatusCode(500, "An unknown error occurred")
        };
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out successfully" });
    }

    /*
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok(new { username, userId });
    }
    */

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