using aresu_txt_editor_backend.Data;
using aresu_txt_editor_backend.Interfaces;
using aresu_txt_editor_backend.Models.Dtos;
using aresu_txt_editor_backend.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace aresu_txt_editor_backend.Services;

public class UserService(ILogger<UserService> _logger, MssqlDbContext _dbContext) : IUserService
{
    private readonly PasswordHasher<string> _hasher = new();

    public LoginResult TryLoginUser(string username, string password, out int userId)
    {
        userId = -1;
        try
        {
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Login failed: User {username} not found", username);
                }

                return LoginResult.UserNotFound;
            }

            var verificationResult = _hasher.VerifyHashedPassword(username, user.PasswordHash, password);
            
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Login failed: Invalid credentials for user {username}", username);
                }

                return LoginResult.InvalidCredentials;
            } 
            
            userId = user.Id;
            
            if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                // Rehash password and update in database
                var newHashedPassword = _hasher.HashPassword(username, password);
                user.PasswordHash = newHashedPassword;
                _dbContext.SaveChanges();

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Password for user {username} rehashed successfully", username);
                }
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("User {username} logged in successfully", username);
            }

            return LoginResult.Success;
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, "An error occurred while logging in user {username}", username);
            }

            return LoginResult.UnknownError;
        }
    }

    public RegisterResult RegisterUser(RegisterDto registerDto)
    {
        try
        {
            // Check if username already exists
            var existingUser = _dbContext.Users
                .FirstOrDefault(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Registration failed: Username {username} already exists",
                        registerDto.Username);
                }

                return RegisterResult.UsernameAlreadyExists;
            }

            // Hash the password
            var hashedPassword = _hasher.HashPassword(registerDto.Username, registerDto.Password);

            // Create new user entity
            var newUser = new Models.User
            {
                Username = registerDto.Username,
                PasswordHash = hashedPassword
            };

            // Add user to database
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("User {username} registered successfully", registerDto.Username);
            }

            return RegisterResult.Success;
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, "An error occurred while registering user {username}", registerDto.Username);
            }

            return RegisterResult.UnknownError;
        }
    }
}