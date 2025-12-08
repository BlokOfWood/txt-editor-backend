using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aresu_txt_editor_backend.Models;

public class TextDocument
{
   [Key]
   [Required]
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
   public int Id { get; set; }
   
   [Required]
   [MaxLength(255)]
   public required string? Title { get; set; }
   
   // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
   public required string? Content { get; set; }
}