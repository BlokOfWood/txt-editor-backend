using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aresu_txt_editor_backend.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; init; }
    [Required]
    [MaxLength(63)]
    public required string Username { get; init; }

    [Required]
    [MaxLength(255)]
    public required string PasswordHash { get; set; }
}