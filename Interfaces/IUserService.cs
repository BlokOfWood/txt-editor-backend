using aresu_txt_editor_backend.Models.Dtos;
using aresu_txt_editor_backend.Models.Enums;

namespace aresu_txt_editor_backend.Interfaces;

public interface IUserService
{ 
    LoginResult TryLoginUser(string username, string password, out int userId);
    RegisterResult RegisterUser(RegisterDto registerDto);
}