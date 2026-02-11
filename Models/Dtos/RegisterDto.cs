using System.ComponentModel.DataAnnotations;

namespace aresu_txt_editor_backend.Models.Dtos;

public class RegisterDto
{
    [StringLength(63)]
    public required string Username { get; set; }
    public required string Password { get; set; }
}